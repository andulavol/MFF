import tap
import sys
import json

okCount = 0
skippedCount = 0
failedCount = 0
i = 0

parser = tap.parser.Parser()
if len(sys.argv) > 0:
    for file in sys.argv:
        for line in parser.parse_file(file):    
            if line.category == 'test':
                if line.ok:
                    okCount += 1
                elif line.skip:
                    skippedCount += 1
                else:
                    failedCount += 1
                i += 1
        data = {"summary":{"filename": file, "total": str(i), "passed" : str(okCount), "skipped": str(skippedCount), "failed": str(failedCount)}}
        json_data = json.dumps(data)
        print(json_data)
        okCount, skippedCount, failedCount, i = 0,0,0,0
