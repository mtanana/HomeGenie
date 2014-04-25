#!/bin/sh
if [ -f /usr/bin/mono ]
then
	MONO=/usr/bin/mono
else
	MONO=/usr/local/bin/mono
fi

EXITCODE="1"
while [ "$EXITCODE" = "1" ]; do
	if [ -z "$1" ]
	then
		sudo $MONO HomeGenie.exe 
	else
		cd $1
		sudo $MONO HomeGenie.exe >/dev/null  2>&1 
	fi
	EXITCODE="$?"
	echo "Exit code: $EXITCODE"
done
