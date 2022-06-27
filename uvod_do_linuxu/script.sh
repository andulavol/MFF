set -ueo pipefail

sed 's#\([0-9][0-9]\):\([0-9][0-9]\)AM#\1: \2#g'
sed 's#\([0-9][0-9]\):\([0-9][0-9]\)PM#\1: \2#g'

while read line
do
  echo "$line"
done < "${1:-/dev/stdin}"