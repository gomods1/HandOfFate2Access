"""Set string fields in one translation table (HandOfFate2Access.Core/Localization/Tables).

usage: python tools/Set-Translations.py <StringTable.XX.cs> <values.json>

values.json maps field name -> translated text (plain text; quotes and backslashes are
escaped here). A field already in the table has its line replaced in place; a new one is
appended before the initializer's closing brace. The file's line endings are kept.
"""
import json
import re
import sys


def literal(text):
    return '"' + text.replace('\\', '\\\\').replace('"', '\\"') + '"'


def main():
    path, values_path = sys.argv[1], sys.argv[2]
    raw = open(path, encoding='utf-8', newline='').read()
    crlf = '\r\n' in raw
    text = raw.replace('\r\n', '\n')
    values = json.load(open(values_path, encoding='utf-8'))
    added = []
    for field, value in values.items():
        line = '\t\t\t%s = %s,' % (field, literal(value))
        pattern = re.compile(r'^\t\t\t' + re.escape(field) + r'\s*=.*,$', re.M)
        if pattern.search(text):
            text = pattern.sub(lambda m: line, text, count=1)
        else:
            added.append(line)
    if added:
        close = text.rindex('\t\t};')
        text = text[:close] + '\n'.join(added) + '\n' + text[close:]
    if crlf:
        text = text.replace('\n', '\r\n')
    open(path, 'w', encoding='utf-8', newline='').write(text)
    print('%s: %d replaced, %d added' % (path, len(values) - len(added), len(added)))


main()
