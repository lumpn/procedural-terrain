Procedural Terrain
==================

A basic Unity project demonstrating how to generate smooth terrain from a density function.

> Conceptually, the terrain surface can be completely described by a single function, called the density function. For any point in 3D space (x, y, z), the function produces a single floating-point value. These values vary over space—sometimes positive, sometimes negative. If the value is positive, then that point in space is inside the solid terrain. If the value is negative, then that point is located in empty space (such as air or water). The boundary between positive and negative values—where the density value is zero—is the surface of the terrain. It is along this surface that we wish to construct a polygonal mesh.

> [Generating Complex Procedural Terrains Using the GPU](http://http.developer.nvidia.com/GPUGems3/gpugems3_ch01.html) - GPU Gems 3

Please note that this is work in progress! The code is far from production ready and huge structural changes will occur.
