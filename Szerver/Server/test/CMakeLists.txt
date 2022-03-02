

add_library (main-t "main-t.cpp")

target_link_libraries(main-t
	PUBLIC Qt5::Core
	PUBLIC Catch2
)

target_include_directories(main-t
	INTERFACE ${CMAKE_CURRENT_SOURCE_DIR}
)