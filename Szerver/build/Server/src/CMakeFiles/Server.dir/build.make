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

# Include any dependencies generated for this target.
include Server/src/CMakeFiles/Server.dir/depend.make
# Include any dependencies generated by the compiler for this target.
include Server/src/CMakeFiles/Server.dir/compiler_depend.make

# Include the progress variables for this target.
include Server/src/CMakeFiles/Server.dir/progress.make

# Include the compile flags for this target's objects.
include Server/src/CMakeFiles/Server.dir/flags.make

Server/src/CMakeFiles/Server.dir/main.cpp.o: Server/src/CMakeFiles/Server.dir/flags.make
Server/src/CMakeFiles/Server.dir/main.cpp.o: ../Server/src/main.cpp
Server/src/CMakeFiles/Server.dir/main.cpp.o: Server/src/CMakeFiles/Server.dir/compiler_depend.ts
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/CMakeFiles --progress-num=$(CMAKE_PROGRESS_1) "Building CXX object Server/src/CMakeFiles/Server.dir/main.cpp.o"
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/src && /usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -MD -MT Server/src/CMakeFiles/Server.dir/main.cpp.o -MF CMakeFiles/Server.dir/main.cpp.o.d -o CMakeFiles/Server.dir/main.cpp.o -c /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/Server/src/main.cpp

Server/src/CMakeFiles/Server.dir/main.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/Server.dir/main.cpp.i"
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/src && /usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/Server/src/main.cpp > CMakeFiles/Server.dir/main.cpp.i

Server/src/CMakeFiles/Server.dir/main.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/Server.dir/main.cpp.s"
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/src && /usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/Server/src/main.cpp -o CMakeFiles/Server.dir/main.cpp.s

# Object files for target Server
Server_OBJECTS = \
"CMakeFiles/Server.dir/main.cpp.o"

# External object files for target Server
Server_EXTERNAL_OBJECTS =

Server/src/Server: Server/src/CMakeFiles/Server.dir/main.cpp.o
Server/src/Server: Server/src/CMakeFiles/Server.dir/build.make
Server/src/Server: Server/src/CMakeFiles/Server.dir/link.txt
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --bold --progress-dir=/mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/CMakeFiles --progress-num=$(CMAKE_PROGRESS_2) "Linking CXX executable Server"
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/src && $(CMAKE_COMMAND) -E cmake_link_script CMakeFiles/Server.dir/link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
Server/src/CMakeFiles/Server.dir/build: Server/src/Server
.PHONY : Server/src/CMakeFiles/Server.dir/build

Server/src/CMakeFiles/Server.dir/clean:
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/src && $(CMAKE_COMMAND) -P CMakeFiles/Server.dir/cmake_clean.cmake
.PHONY : Server/src/CMakeFiles/Server.dir/clean

Server/src/CMakeFiles/Server.dir/depend:
	cd /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/Server/src /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/src /mnt/c/Projektek/Karbantart-s-menedzsment/Szerver/build/Server/src/CMakeFiles/Server.dir/DependInfo.cmake --color=$(COLOR)
.PHONY : Server/src/CMakeFiles/Server.dir/depend
