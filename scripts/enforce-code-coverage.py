import argparse
import glob
import sys
import xml.etree.ElementTree as ET

def main() -> int:
    parser = argparse.ArgumentParser(description = "Fail if coverage.cobertura is below line/threshold.")
    parser.add_argument("--threshold", type = float, default = 0.60, help = "Min line and branch coverage as fraction (default: 0.60)",)
    args = parser.parse_args()

    if not 0.0 <= args.threshold <= 1.0:
        parser.error("--threshold must be between 0.0 and 1.0")

    matches = glob.glob("**/TestResults/**/coverage.cobertura.xml", recursive = True)
    if not matches:
        print(
            "ERROR: No coverage.cobertura found. "
            "Did Tests run with --settings coverlet.runsettings?",
            file = sys.stderr)
        return 1

    root = ET.parse(matches[0]).getroot()

    if int(root.get("lines-valid","0")) == 0:
        print("ERROR: Nothing was instrumented (lines-valid = 0), coverage is empty.",file = sys.stderr)
        return 1

    line_rate = float(root.get("line-rate", "0"))
    branch_rate = float(root.get("branch-rate", "0"))
    threshold = args.threshold

    failures = []
    if line_rate < threshold:
        failures.append(f"Line Coverage {line_rate:.1%} below {threshold:.0%}")
    if branch_rate < threshold:
        failures.append(f"Branch Coverage {branch_rate:.1%} below {threshold:.0%}")

    status = "FAIL" if failures else "PASS"
    print(f"Line {line_rate:.1%} | Branch {branch_rate:.1%} | Threshold {threshold:.0%} -> {status}")
    for failure in failures:
        print(f"    {failure}")

    return 1 if failures else 0

if __name__ == "__main__":
    sys.exit(main())