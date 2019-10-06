#!/bin/sh

found=0

if [ "$1" = "Win-x64" ] || [ "$1" = "Linux-x64" ] || [ "$1" = "Mac-x64" ]; then
	found=1
fi

if [ $found -eq 0 ]; then
	printf "\n[91mInvalid platform specified![0m Valid options are: [94mLinux-64[0m, [94mWin-x64[0m or [94mMac-x64[0m\n\n"
	exit 64
fi

platform=$(echo "$1" | tr '[:upper:]' '[:lower:]')

printf "\nBuilding with target [94m$1[0m\n"

printf "\n"
printf "[104;97mDeleting previous build...[0m\n"

rm -rf Release/$platform/*

printf "\n"
printf "[104;97mCopying common resources...[0m\n"

cp -r Resources/$platform/* Release/$platform/

printf "\n"
printf "[104;97mBuilding ScriptConsole...[0m\n"

cd ScriptConsole
dotnet publish -c Release -f netcoreapp3.0 /p:PublishProfile=Properties/PublishProfiles/$platform.pubxml
cd ..

printf "\n"
printf "[94mAll done![0m\n\n"
