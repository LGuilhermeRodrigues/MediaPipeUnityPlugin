load("@rules_pkg//:pkg.bzl", "pkg_zip")
load("//mediapipe_api:import_model.bzl", "pkg_asset")

cc_library(
    name = "mediapipe_c",
    deps = [
        ":calculators",
        "//mediapipe_api/external:glog",
        "//mediapipe_api/external:protobuf",
        "//mediapipe_api/external:stdlib",
        "//mediapipe_api/framework:calculator",
        "//mediapipe_api/framework:calculator_graph",
        "//mediapipe_api/framework:output_stream_poller",
        "//mediapipe_api/framework:timestamp",
        "//mediapipe_api/framework/formats:classification",
        "//mediapipe_api/framework/formats:detection",
        "//mediapipe_api/framework/formats:image_frame",
        "//mediapipe_api/framework/formats:landmark",
        "//mediapipe_api/framework/formats:rect",
        "//mediapipe_api/framework/port:logging",
        "//mediapipe_api/graphs/object_detection_3d/calculators:model_matrix",
        "//mediapipe_api/modules/face_geometry/protos:face_geometry",
        "//mediapipe_api/util:resource_util",
    ] + select({
        "@com_google_mediapipe//mediapipe/gpu:disable_gpu": [],
        "//conditions:default": [
            "//mediapipe_api/gpu:gl_base",
            "//mediapipe_api/gpu:gl_context",
            "//mediapipe_api/gpu:gl_calculator_helper",
            "//mediapipe_api/gpu:gl_texture_buffer",
            "//mediapipe_api/gpu:gpu_buffer",
            "//mediapipe_api/gpu:gpu_buffer_format",
            "//mediapipe_api/gpu:gpu_shared_data_internal",
        ],
    }) + select({
        "@com_google_mediapipe//mediapipe/gpu:disable_gpu": [],
        "@com_google_mediapipe//mediapipe:ios": [],
        "//conditions:default": [
            "//mediapipe_api/gpu:egl_surface_holder",
        ],
    }),
    visibility = ["//visibility:public"],
    alwayslink = True,
)

cc_binary(
    name = "libmediapipe_c.so",
    deps = [":mediapipe_c"],
    linkshared = True,
    linkstatic = True,
    visibility = ["//visibility:public"],
)

cc_binary(
    name = "libmediapipe_c.dylib",
    deps = [":mediapipe_c"],
    linkshared = True,
    linkstatic = True,
    visibility = ["//visibility:public"],
)

cc_binary(
    name = "libmediapipe_c.dll",
    deps = [":mediapipe_c"],
    linkshared = True,
    linkstatic = True,
    visibility = ["//visibility:public"],
)

alias(
    name = "libmediapipe_c",
    actual = select({
        "@com_google_mediapipe//mediapipe:macos": ":libmediapipe_c.dylib",
        "@com_google_mediapipe//mediapipe:windows": ":libmediapipe_c.dll",
        "//conditions:default": ":libmediapipe_c.so",
    }),
)

# It is recommended that you include only neccessary calculators.
cc_library(
    name = "calculators",
    deps = [
        "@com_google_mediapipe//mediapipe/calculators/core:pass_through_calculator",
        "@com_google_mediapipe//mediapipe/calculators/core:packet_presence_calculator",
        "@com_google_mediapipe//mediapipe/calculators/core:flow_limiter_calculator",
        "@com_google_mediapipe//mediapipe/calculators/image:image_transformation_calculator",
    ] + select({
        "@com_google_mediapipe//mediapipe/gpu:disable_gpu": [
            "@com_google_mediapipe//mediapipe/graphs/pose_tracking:pose_tracking_cpu_deps",
            "@com_google_mediapipe//mediapipe/graphs/tracking:desktop_calculators",
        ],
        "//conditions:default": [
            # ATTENTION!
            #   If you're building for Android with debug symobls (--compilation_mode=dbg), exclude `instant_motion_tracking_deps`
            #   or exclude the other deps defined under `mediapipe/graphs`.
            #   Otherwise the app will crash (MediaPipe may have some bugs).
            "@com_google_mediapipe//mediapipe/graphs/pose_tracking:pose_tracking_gpu_deps",
            "@com_google_mediapipe//mediapipe/graphs/tracking:mobile_calculators",
        ],
    }),
)

cc_library(
    name = "common",
    srcs = ["common.cc"],
    hdrs = ["common.h"],
    deps = [
        "@com_google_mediapipe//mediapipe/framework/port:logging",
    ],
    visibility = ["//visibility:public"],
    alwayslink = True,
)

pkg_zip(
    name = "mediapipe_desktop",
    srcs = [":libmediapipe_c"],
)

pkg_asset(
    name = "mediapipe_assets",
    srcs = [
        "@com_google_mediapipe//mediapipe/modules/pose_detection:pose_detection.tflite",
        "@com_google_mediapipe//mediapipe/modules/pose_landmark:pose_landmark_full.tflite",
    ],
)

pkg_zip(
    name = "mediapipe_proto_srcs",
    srcs = [
        "//mediapipe_api/framework:calculator_cs",
        "//mediapipe_api/framework:calculator_options_cs",
        "//mediapipe_api/framework:mediapipe_options_cs",
        "//mediapipe_api/framework:packet_factory_cs",
        "//mediapipe_api/framework:packet_generator_cs",
        "//mediapipe_api/framework:status_handler_cs",
        "//mediapipe_api/framework:stream_handler_cs",
        "//mediapipe_api/framework/formats:classification_cs",
        "//mediapipe_api/framework/formats:detection_cs",
        "//mediapipe_api/framework/formats:landmark_cs",
        "//mediapipe_api/framework/formats:location_data_cs",
        "//mediapipe_api/framework/formats:matrix_data_cs",
        "//mediapipe_api/framework/formats:rasterization_cs",
        "//mediapipe_api/framework/formats:rect_cs",
        "//mediapipe_api/graphs/instant_motion_tracking/calculators:sticker_buffer_cs",
        "//mediapipe_api/graphs/object_detection_3d/calculators:model_matrix_cs",
        "//mediapipe_api/modules/face_geometry/protos:face_geometry_cs",
        "//mediapipe_api/modules/face_geometry/protos:mesh_3d_cs",
    ],
    # TODO: keep directory structure
)

pkg_zip(
    name = "opencv_libs",
    srcs = select({
        "@opencv//:source_build": ["@opencv//:opencv_libs"],
        "@com_google_mediapipe//mediapipe:windows": ["@windows_opencv//:opencv_libs"],
        "@com_google_mediapipe//mediapipe:macos": [],
        "@com_google_mediapipe//mediapipe:android": [],
        "@com_google_mediapipe//mediapipe:ios": [],
        "//conditions:default": ["@linux_opencv//:opencv_libs"],
    }),
)
