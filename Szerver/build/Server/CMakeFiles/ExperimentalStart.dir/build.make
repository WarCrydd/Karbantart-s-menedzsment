# CMAKE generated file: DO NOT EDIT!
# Generated by "Unix Makefiles" Generator, CMake Version 3.20

# Delete rule output on recipe failure.
.DELETE_ON_ERROR:

#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:

# Disable VCS-based implicit rules.
% : %,v

# Disable VCS-based implicit rules.
% : RCS/%

# Disable VCS-based implicit rules.
% : RCS/%,v

# Disable VCS-based implicit rules.
% : SCCS/s.%

# Disable VCS-based implicit rules.
% : s.%

.SUFFIXES: .hpux_make_needs_suffix_list

# Command-line flag to silence nested $(MAKE).
$(VERBOSE)MAKESILENT = -s

#Suppress display of executed commands.
$(VERBOSE).SILENT:

# A target that is always out of date.
cmake_force:
.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

# The shell in which to execute make rules.
SHELL = /bin/sh

# The CMake executable.
CMAKE_COMMAND = /usr/local/bin/cmake

# The command to remove a file.
RM = /usr/local/bin/cmake -E rm -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build

# Utility rule file for ExperimentalStart.

# Include any custom commands dependencies for this target.
include Server/CMakeFiles/ExperimentalStart.dir/compiler_depend.make

# Include the progress variables for this target.
include Server/CMakeFiles/ExperimentalStart.dir/progress.make

Server/CMakeFiles/ExperimentalStart:
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server && /usr/local/bin/ctest -D ExperimentalStart

ExperimentalStart: Server/CMakeFiles/ExperimentalStart
ExperimentalStart: Server/CMakeFiles/ExperimentalStart.dir/build.make
.PHONY : ExperimentalStart

# Rule to build all files generated by this target.
Server/CMakeFiles/ExperimentalStart.dir/build: ExperimentalStart
.PHONY : Server/CMakeFiles/ExperimentalStart.dir/build

Server/CMakeFiles/ExperimentalStart.dir/clean:
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server && $(CMAKE_COMMAND) -P CMakeFiles/ExperimentalStart.dir/cmake_clean.cmake
.PHONY : Server/CMakeFiles/ExperimentalStart.dir/clean

Server/CMakeFiles/ExperimentalStart.dir/depend:
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/Server /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/CMakeFiles/ExperimentalStart.dir/DependInfo.cmake --color=$(COLOR)
.PHONY : Server/CMakeFiles/ExperimentalStart.dir/depend

