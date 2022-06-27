#!/bin/bash

set -ueo pipefail

echo "Usage: sysinfo [options]
 -c   --cpu     Print number of CPUs.
 -l   --load    Print current load.
 -k   --kernel  Print kernel version.
 -s   --script  Each value on separate line." > help.txt
echo >> help.txt
echo "Without arguments behave as with -c -l -k." >> help.txt
echo >> help.txt
echo "Copyright NSWI177 2022" >> help.txt

if [ "$#" == 0 ]; then
    echo -n "load="
    awk -n '{ printf $1 " " }' /proc/loadavg    
    echo -n "kernel=$(uname -r) "
    echo "cpus=$(nproc)"
elif [ "$#" == 1 ] && ( [ "$1" = "--script" ] ||  [ "$1" = "-s" ] ) ; then
    echo -n "load="
    awk '{ printf $1 }' /proc/loadavg
    echo
    echo "kernel=$(uname -r)"
    echo "cpus=$(nproc)"
elif [ "$*" = "-s" ] || [ "$*" = "--script" ] ; then
    for i; do
	if [ "$i" = "--load" ] ; then
	    echo -n "load="
	    awk '{ printf $1 " " }' /proc/loadavg
	elif [ "$i" = "--kernel" ] ; then
	    echo "kernel=$(uname -r) "
	elif [ "$i" = "--cpus" ] ; then
	    echo -n "cpus=$(nproc)"	
	elif [ "$i" = "--help" ] ; then
	    cat help.txt
	fi
    done
    for i; do
	if [ "$i" = "-f" ] ; then
	    echo -n "load="
	    awk '{ printf $1 " " }' /proc/loadavg
	elif [ "$i" = "-k" ] ; then
	    echo "kernel=$(uname -r) "
	elif [ "$i" = "-c" ] ; then
	    echo -n "cpus=$(nproc)"	
	elif [ "$i" = "-h" ] ; then
	    cat help.txt
	fi
     done
else
    for i; do
	if [ "$i" = "--load" ] ; then
	    echo -n "load="
	    awk -n '{ printf $1 " " }' /proc/loadavg
	elif [ "$i" = "--kernel" ] ; then
	    echo -n "kernel=$(uname -r) "
	elif [ "$i" = "--cpus" ] ; then
	    echo "cpus=$(nproc)"	
	elif [ "$i" = "--help" ] ; then
	    cat help.txt
	fi
    done
    for i; do
	if [ "$i" = "-f" ] ; then
	    echo -n "load="
	    awk -n '{ printf $1 " " }' /proc/loadavg
	elif [ "$i" = "-k" ] ; then
	    echo -n "kernel=$(uname -r)"
	elif [ "$i" = "-c" ] ; then
	    echo "cpus=$(nproc)"	
	elif [ "$i" = "-h" ] ; then
	    cat help.txt
	fi
    done    
fi
