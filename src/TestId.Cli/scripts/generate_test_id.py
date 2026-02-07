#!/usr/bin/env python3
"""
Simple test ID generator script.
Generates a random UUID as a test ID.
"""
import argparse
import uuid
import sys

PREFIXES = {
    "U": "UT",
    "C": "AT",
}

def generate_test_id(kind):
    """Generate a unique test ID with a prefix based on kind."""
    prefix = PREFIXES.get(kind, "UT")
    test_id = f"{prefix}-{uuid.uuid4()}"
    return test_id

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Generate a test ID")
    parser.add_argument("-kind", choices=["U", "C"], default="U",
                        help="Kind of test: U for unit test, C for acceptance test")
    args = parser.parse_args()

    test_id = generate_test_id(args.kind)
    print(test_id)
    sys.exit(0)
