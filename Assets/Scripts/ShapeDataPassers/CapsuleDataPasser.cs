using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleDataPasser : BaseShapeDataPasser {
    [Min(0)]
    public float height;
    [Min(0)]
    public float radius;

    private int[] meshTriangles;

    protected override void AssignMesh() {
        mesh = CreateMesh();
        base.AssignMesh();
    }

    private Mesh CreateMesh() {
        /* numSegmentsCylinder is how many points make up one end of the cylinder
         * numSegments Hemisphere is the number of concentric rings there will be, since the last one isn't a ring, it's a single point
         * the first coencentric ring of each hemisphere will be one of the 2 rings that make up the cylinder
         */

        Mesh mesh = new Mesh();

        //vertices
        int numSegmentsCylinder = 50;
        int numSegmentsHemisphere = 10;
        Vector3[] vertices = new Vector3[(numSegmentsHemisphere * numSegmentsCylinder + 1) * 2];

        float cylinderAngleIncrement = 360f / numSegmentsCylinder;

        float hemisphereInclinationAngleIncrement = 90f / numSegmentsHemisphere;
        int nextVertexIndex = 0;
        // the hemispheres
        for (int i = 0; i < 2; i++) {
            //whether we're working on the upper or lower hemisphere
            int ySign = i % 2 == 0 ? 1 : -1;
            Vector3 hemisphereCenter = ySign * Vector3.up * height / 2f;
            for (int j = 0; j < numSegmentsHemisphere; j++) {
                float hemisphereAngle = j * hemisphereInclinationAngleIncrement;
                for (int k = 0; k < numSegmentsCylinder; k++) {
                    float angle = k * cylinderAngleIncrement;
                    Vector3 zAxisRotatedPoint = Quaternion.AngleAxis(hemisphereAngle, Vector3.forward) * Vector3.right * radius * ySign;
                    Vector3 finalHemispherePoint = Quaternion.AngleAxis(angle + (ySign == -1 ? 180f : 0), Vector3.up) * zAxisRotatedPoint;
                    vertices[nextVertexIndex] = hemisphereCenter + finalHemispherePoint;
                    nextVertexIndex++;
                }
            }
            vertices[nextVertexIndex] = hemisphereCenter + Vector3.up *  radius * ySign;
            nextVertexIndex++;
        }

        /* numTrisCylinder = 2 * numSegmentsCylinder
         * numTrisHemisphere = 2 * (numSegmentsHemisphere - 1) * numSegmentsCylinder + numSegmentsCylinder 
         * = 2 * numSegmentsHemisphere * numSegmentsCylinder - 2 * numSegmentsCylinder + numSegmentsCylinder
         * = 2 * numSegmentsHemisphere * numSegmentsCylinder - numSegmentsCylinder = numSegmentsCylinder * (2 * numSegmentsHemisphere - 1)
         * numTriangles = numTrisCylinder + 2 * numTrisHemisphere = 2 * numSegmentsCylinder + 2 * numTrisHemisphere = 2 * (numSegmentsCylinder + numTrisHemisphere)
         */
        int numTrisHemisphere = numSegmentsCylinder * (2 * numSegmentsHemisphere - 1);
        int numTriangles = 2 * (numSegmentsCylinder + numTrisHemisphere);

        //triangles
        if (meshTriangles == null || mesh.triangles.Length == 0 || meshTriangles.Length != numTriangles * 3) {
            /* numTrisCylinder = 2 * numSegmentsCylinder
             * numTrisHemisphere = 2 * (numSegmentsHemisphere - 1) * numSegmentsCylinder + numSegmentsCylinder 
             * = 2 * numSegmentsHemisphere * numSegmentsCylinder - 2 * numSegmentsCylinder + numSegmentsCylinder
             * = 2 * numSegmentsHemisphere * numSegmentsCylinder - numSegmentsCylinder = numSegmentsCylinder * (2 * numSegmentsHemisphere - 1)
             * numTriangles = numTrisCylinder + 2 * numTrisHemisphere = 2 * numSegmentsCylinder + 2 * numTrisHemisphere = 2 * (numSegmentsCylinder + numTrisHemisphere)
             */
            int numVerticesHemisphere = 1 + numSegmentsHemisphere * numSegmentsCylinder;
            meshTriangles = new int[numTriangles * 3];

            int triIndex = 0;
            //the cylinder
            for (int i = 0; i < numSegmentsCylinder; i ++) {
                //if you're the last vertex on that section of ring, you have to subtract to get to the first one
                int nextVertexOnSameCircleOffset = (i + 1) % numSegmentsCylinder == 0 ? -(numSegmentsCylinder - 1) : 1;

                meshTriangles[triIndex] = i;
                meshTriangles[triIndex + 1] = i + numVerticesHemisphere;
                meshTriangles[triIndex + 2] = i + numVerticesHemisphere + nextVertexOnSameCircleOffset;
                triIndex += 3;
                
                meshTriangles[triIndex] = i;
                meshTriangles[triIndex + 1] = i + numVerticesHemisphere + nextVertexOnSameCircleOffset;
                meshTriangles[triIndex + 2] = i + nextVertexOnSameCircleOffset;
                triIndex += 3;
            }

            //the hemispheres
            for (int i = 0; i < 2; i++) {
                int vertexStartIndex = i * numVerticesHemisphere;
                for (int j = 0; j < numSegmentsHemisphere * numSegmentsCylinder; j++) {
                //if you're the last vertex on that section of ring, you have to subtract to get to the first one
                int nextVertexOnSameCircleOffset = (j + 1) % numSegmentsCylinder == 0 ? -(numSegmentsCylinder - 1) : 1;
                    if (j / numSegmentsCylinder == numSegmentsHemisphere - 1) {
                        //last coencentric circle
                        meshTriangles[triIndex] = vertexStartIndex + j;
                        meshTriangles[triIndex + 1 + i] = vertexStartIndex + j + nextVertexOnSameCircleOffset;
                        meshTriangles[triIndex + 2 - i] = (i + 1) * numVerticesHemisphere - 1;
                        triIndex += 3;
                    } else {
                        meshTriangles[triIndex] = vertexStartIndex + j;
                        meshTriangles[triIndex + 1 + i] = vertexStartIndex + j + nextVertexOnSameCircleOffset;
                        meshTriangles[triIndex + 2 - i] = vertexStartIndex + j + numSegmentsCylinder + nextVertexOnSameCircleOffset;
                        triIndex += 3;

                        meshTriangles[triIndex] = vertexStartIndex + j;
                        meshTriangles[triIndex + 1 + i] = vertexStartIndex + j + numSegmentsCylinder + nextVertexOnSameCircleOffset;
                        meshTriangles[triIndex + 2 - i] = vertexStartIndex + j + numSegmentsCylinder;
                        triIndex += 3;
                    }
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    protected override Vector3 GetInfo() {
        return new Vector3(height, radius);
    }

    protected override ShapeType GetShapeType() {
        return ShapeType.capsule;
    }
}
