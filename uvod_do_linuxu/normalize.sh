#!/bin/bash
set -ueo pipefail

function normalize()
{
  local   path=${1//\/.\//\/}
    local   inpath=$(echo $path | sed -e 's;[^/][^/]*/\.\./;;g')
    while [[ $inpath != "$path" ]]
    do
        path=$inpath
        inpath=$(echo $path | sed -e 's;[^/][^/]*/\.\./;;g')
    done
    echo "$path"
}

if [[ $(basename "$(basename """$0""" .sh)" .old) == 'normalize' ]]; then
    if [[ $* ]]; then
        for p in "$@"
        do
            printf "%s$(normalize $p)"
        done
    fi
fi