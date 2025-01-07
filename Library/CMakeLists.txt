cmake_minimum_required(VERSION 3.20)
project(tracking)

set(CMAKE_CXX_STANDARD 20)

# Include directory for openvr and local includes
include_directories("${CMAKE_CURRENT_SOURCE_DIR}/openvr/include")
include_directories("${CMAKE_CURRENT_SOURCE_DIR}/includes")  # Include local headers

# Link directory for openvr
link_directories("${CMAKE_CURRENT_SOURCE_DIR}/openvr/lib")

# Add src files to build
file(GLOB_RECURSE SOURCES ${PROJECT_SOURCE_DIR}/src/*.cpp)

# Add library target and add files from src
add_library(tracking SHARED ${SOURCES})

set(CMAKE_SHARED_LINKER_FLAGS "-static-libgcc -static-libstdc++")

# Links openvr
target_link_libraries(tracking openvr_api)

# Copies openvr_api.dll to the binary folder
add_custom_command(
        TARGET tracking PRE_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy
                ${CMAKE_SOURCE_DIR}/openvr/lib/openvr_api.dll
                ${CMAKE_BINARY_DIR}/openvr_api.dll)
