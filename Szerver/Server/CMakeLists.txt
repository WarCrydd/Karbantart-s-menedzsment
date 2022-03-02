cmake_minimum_required (VERSION 3.8)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(Qt5
	COMPONENTS
	Core
	REQUIRED
)

include(CTest)
enable_testing()

add_subdirectory("src")
add_subdirectory("test")