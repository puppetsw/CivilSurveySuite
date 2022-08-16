# takes and input markdown file with tables and generates a new markdown file for each row in table
# using the first column as the file name and the second column as the contents.

# usage: python docgen.py input.md output_directory


import sys
from os.path import exists


def generate_markdown_file(command_name: str, description: str) -> str:
    md = f"# {command_name.upper()}\n" \
         f"\n" \
         f"## Description\n" \
         f"\n" \
         f"{description}\n" \
         f"\n" \
         f"## Usage\n" \
         f"\n" \
         f"## Example Output\n" \
         f"```\n" \
         f"```\n"
    return md


def main():
    if len(sys.argv) != 3:
        print("usage: python docgen.py input.md output_dir")
        return

    input_file = sys.argv[1]
    output_dir = sys.argv[2]

    if not exists(input_file):
        print("input file does not exist")
        return

    if not exists(output_dir):
        print("output directory does not exist")
        return

    with open(input_file, "r") as f:
        lines = f.readlines()

    for line in lines:
        if not line.startswith('|'):
            continue

        parts = line.split('|')[1:3]
        if len(parts) != 2 or ' --- ' in parts:
            continue

        # works, but a bit hacky.
        if parts[0].strip() == 'Command' and parts[1].strip() == 'Description':
            continue

        filename = parts[0].strip()
        contents = parts[1].strip()

        with open(output_dir + "/" + filename.upper() + ".md", "w") as f:
            f.write(generate_markdown_file(filename, contents))


if __name__ == "__main__":
    main()
