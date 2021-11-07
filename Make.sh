#!/bin/bash

if [ -t 1 ]; then
    ANSI_RESET="$(tput sgr0)"
    ANSI_UNDERLINE="$(tput smul)"
    ANSI_RED="`[ $(tput colors) -ge 16 ] && tput setaf 9 || tput setaf 1 bold`"
    ANSI_YELLOW="`[ $(tput colors) -ge 16 ] && tput setaf 11 || tput setaf 3 bold`"
    ANSI_CYAN="`[ $(tput colors) -ge 16 ] && tput setaf 14 || tput setaf 6 bold`"
    ANSI_WHITE="`[ $(tput colors) -ge 16 ] && tput setaf 15 || tput setaf 7 bold`"
fi

while getopts ":h" OPT; do
    case $OPT in
        h)
            echo
            echo    "  SYNOPSIS"
            echo -e "  $(basename "$0") [${ANSI_UNDERLINE}operation${ANSI_RESET}]"
            echo
            echo -e "    ${ANSI_UNDERLINE}operation${ANSI_RESET}"
            echo    "    Operation to perform."
            echo
            echo    "  DESCRIPTION"
            echo    "  Make script compatible with both Windows and Linux."
            echo
            echo    "  SAMPLES"
            echo    "  $(basename "$0")"
            echo    "  $(basename "$0") dist"
            echo
            exit 0
        ;;

        \?) echo "${ANSI_RED}Invalid option: -$OPTARG!${ANSI_RESET}" >&2 ; exit 1 ;;
        :)  echo "${ANSI_RED}Option -$OPTARG requires an argument!${ANSI_RESET}" >&2 ; exit 1 ;;
    esac
done

if [[ -f ~/dotnet/dotnet ]]; then
    DOTNET_CMD=~/dotnet/dotnet
else
    DOTNET_CMD=dotnet
fi

if ! command -v $DOTNET_CMD >/dev/null; then
    echo "${ANSI_RED}No dotnet found!${ANSI_RESET}" >&2
    exit 1
fi

trap "exit 255" SIGHUP SIGINT SIGQUIT SIGPIPE SIGTERM
trap "echo -n \"$ANSI_RESET\"" EXIT

BASE_DIRECTORY="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"


function clean() {
    rm -r "$BASE_DIRECTORY/bin/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/build/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/src/**/bin/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/src/**/obj/" 2>/dev/null
    return 0
}

function distclean() {
    rm -r "$BASE_DIRECTORY/dist/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/target/" 2>/dev/null
    return 0
}

function dist() {
    DIST_DIRECTORY="$BASE_DIRECTORY/build/dist/$APP_NAME-$APP_VERSION"
    DIST_FILE=
    rm -r "$DIST_DIRECTORY/" 2>/dev/null
    mkdir -p "$DIST_DIRECTORY/"
    for DIRECTORY in "Make.sh" "CONTRIBUTING.md" "ICON.png" "LICENSE.md" "README.md" "src"; do
        cp -r "$BASE_DIRECTORY/$DIRECTORY" "$DIST_DIRECTORY/"
    done
    find "$DIST_DIRECTORY/src/" -name ".vs" -type d -exec rm -rf {} \; 2>/dev/null
    find "$DIST_DIRECTORY/src/" -name "bin" -type d -exec rm -rf {} \; 2>/dev/null
    find "$DIST_DIRECTORY/obj/" -name "bin" -type d -exec rm -rf {} \; 2>/dev/null
    tar -cz -C "$BASE_DIRECTORY/build/dist/" \
        --owner=0 --group=0 \
        -f "$DIST_DIRECTORY.tar.gz" \
        "$APP_NAME-$APP_VERSION/" || return 1
    mkdir -p "$BASE_DIRECTORY/dist/"
    mv "$DIST_DIRECTORY.tar.gz" "$BASE_DIRECTORY/dist/" || return 1
    echo "${ANSI_CYAN}Archive in 'dist/$APP_NAME-$APP_VERSION.tar.gz'${ANSI_RESET}"
    return 0
}

function debug() {
    mkdir -p "$BASE_DIRECTORY/bin/"
    mkdir -p "$BASE_DIRECTORY/build/debug/"
    $DOTNET_CMD build --configuration "Debug" \
                      --output "$BASE_DIRECTORY/build/debug/" \
                      --verbosity "minimal" \
                      "$BASE_DIRECTORY/src/DiskPreclear.sln" || return 1
    cp "$BASE_DIRECTORY/build/debug/$APP_NAME"* "$BASE_DIRECTORY/bin/" || return 1
    echo "${ANSI_CYAN}Binaries in 'bin/'${ANSI_RESET}"
}

function release() {
    if [[ `shell git status -s 2>/dev/null | wc -l` -gt 0 ]]; then
        echo "${ANSI_YELLOW}Uncommited changes present.${ANSI_RESET}" >&2
    fi
    mkdir -p "$BASE_DIRECTORY/bin/"
    mkdir -p "$BASE_DIRECTORY/build/release/"
    $DOTNET_CMD build --configuration "Release" \
                      --output "$BASE_DIRECTORY/build/release/" \
                      --verbosity "minimal" \
                      "$BASE_DIRECTORY/src/DiskPreclear.sln" || return 1
    cp "$BASE_DIRECTORY/build/release/$APP_NAME"* "$BASE_DIRECTORY/bin/" || return 1
    echo "${ANSI_CYAN}Binaries in 'bin/'${ANSI_RESET}"
}

function publish() {
    if [[ `shell git status -s 2>/dev/null | wc -l` -gt 0 ]]; then
        echo "${ANSI_YELLOW}Uncommited changes present.${ANSI_RESET}" >&2
    fi
    if [[ `uname -o` == "Msys" ]]; then  # assume Windows
        PLATFORM="win-x64"
    else
        PLATFORM="linux-x64"
    fi
    mkdir -p "$BASE_DIRECTORY/bin/"
    mkdir -p "$BASE_DIRECTORY/build/publish/"
    $DOTNET_CMD publish --configuration "Release" \
                        --force \
                        --output "$BASE_DIRECTORY/build/publish/" \
                        --self-contained true \
                        --runtime $PLATFORM \
                        -p:Deterministic=true \
                        -p:Optimize=true \
                        -p:PublishSingleFile=true \
                        -p:DebugType=portable \
                        "$BASE_DIRECTORY/src/DiskPreclear.csproj" || return 1
    cp "$BASE_DIRECTORY/build/publish/$APP_NAME"* "$BASE_DIRECTORY/bin/" || return 1
    echo "${ANSI_CYAN}Binaries in 'bin/'${ANSI_RESET}"
}

function test() {
    mkdir -p "$BASE_DIRECTORY/build/test/"
    echo ".NET `dotnet --version`"
    $DOTNET_CMD test --configuration "Debug" \
                     --output "$BASE_DIRECTORY/build/test/" \
                     --verbosity "minimal" \
                     "$BASE_DIRECTORY/src/DiskPreclear.sln" || return 1
}


APP_NAME=`cat "$BASE_DIRECTORY/src/DiskPreclear.csproj" | grep "<AssemblyName>" | sed 's^</\?AssemblyName>^^g' | xargs`
APP_VERSION=`cat "$BASE_DIRECTORY/src/DiskPreclear.csproj" | grep "<Version>" | sed 's^</\?Version>^^g' | xargs`

while [ $# -gt 0 ]; do
    OPERATION="$1"
    case "$OPERATION" in
        all)         clean || break ;;
        clean)       clean || break ;;
        debug)       debug || break ;;
        release)     release || break ;;
        publish)     publish || break ;;
        test)        test || break ;;
        distclean)   distclean || break ;;
        dist)        dist || break ;;

        *)  echo "${ANSI_RED}Unknown operation '$OPERATION'!${ANSI_RESET}" >&2 ; exit 1 ;;
    esac

    shift
done

if [[ "$1" != "" ]]; then
    echo "${ANSI_RED}Error performing '$OPERATION' operation!${ANSI_RESET}" >&2
    exit 1
fi
