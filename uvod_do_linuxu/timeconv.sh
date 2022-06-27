#!/bin/bash
set -ueo pipefail

while read -r line; do
    for word in $line; do
        if [ "${word: -2}" == "AM" ]; then 
            hours=$(echo "$word" | sed "s/\([0-9][0-9]\):\([0-9][0-9]\)\(AM\)/\1/g")
            minutes=$(echo "$word" | sed "s/\([0-9][0-9]\):\([0-9][0-9]\)\(AM\)/\2/g")
            if [ "$hours" == "12" ]; then
                echo -n "00:$minutes"
            else
                echo -n "$hours:$minutes "
            fi
	    elif [ "${word: -2}" == "PM" ]; then
            hours=$(echo "$word" | sed "s/\([0-9][0-9]\):\([0-9][0-9]\)\(PM\)/\1/g")
            minutes=$(echo "$word" | sed "s/\([0-9][0-9]\):\([0-9][0-9]\)\(PM\)/\2/g")
            if [ "$hours" == "12" ]; then
                echo -n "$hours:$minutes "
            else
            echo -n "$((hours + 12)):$minutes "
            fi
        else 
            echo -n "$word "
    fi
    done
    echo '' 
done