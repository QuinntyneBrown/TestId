#!/usr/bin/env python3
"""
Simple test ID generator script.
Generates a random UUID as a test ID.
"""
import uuid
import sys

def generate_test_id():
    """Generate a unique test ID."""
    test_id = str(uuid.uuid4())
    return test_id

if __name__ == "__main__":
    test_id = generate_test_id()
    print(test_id)
    sys.exit(0)
