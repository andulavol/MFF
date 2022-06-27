#!/bin/bash
set -ueo pipefail

function trap_ctrlc ()
{
    echo "Aborted"
    exit 17
}

trap "trap_ctrlc" 2
echo "$1"
sleep 1

for ((i="$1-1";i>=1;i--)); do 
    echo "$i"
    sleep 1 
done