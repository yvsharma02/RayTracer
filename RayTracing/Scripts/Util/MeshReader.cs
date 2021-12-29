using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RayTracing
{
    public class MeshReader
    {
        public static MeshBuilder ReadObj(String location)
        {
            LinkedList<Vector3D> vertices = new LinkedList<Vector3D>();
            LinkedList<Vector2D> uvs = new LinkedList<Vector2D>();
            LinkedList<Vector3D> vertexNormals = new LinkedList<Vector3D>();

            LinkedList<int> triangles = new LinkedList<int>();
            LinkedList<int> uvTriangles = new LinkedList<int>();
            LinkedList<int> normalTriangles = new LinkedList<int>();

            String[] lines = File.ReadAllLines(location);

            bool uvLoadingFailed = false;
            bool normalLoadingFailed = false;

            foreach (String line in lines)
            {
                if (line.StartsWith("v ") || line.StartsWith("vn "))
                {
                    bool areNormals = line.StartsWith("vn ");

                    int[] substringStartIndices = new int[4];
                    int c = 0;

                    bool spaceSeq = false;

                    for (int i = 0; c < 4 && i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            if (!spaceSeq)
                                substringStartIndices[c++] = i;
                            spaceSeq = true;
                        }
                        else
                            spaceSeq = false;
                    }

                    if (c == 3)
                        substringStartIndices[c++] = line.Length;

                    float x = float.Parse(line.Substring(substringStartIndices[0] + 1, substringStartIndices[1] - substringStartIndices[0] - 1));
                    float y = float.Parse(line.Substring(substringStartIndices[1] + 1, substringStartIndices[2] - substringStartIndices[1] - 1));
                    float z = float.Parse(line.Substring(substringStartIndices[2] + 1, substringStartIndices[3] - substringStartIndices[2] - 1));

                    if (!areNormals)
                        vertices.AddLast(new Vector3D(x, y, z));
                    else
                        vertexNormals.AddLast(new Vector3D(x, y, z).Normalize());
                }
                else if (line.StartsWith("vt "))
                {
                    int[] substringStartIndices = new int[3];
                    int c = 0;

                    bool spaceSeq = false;

                    for (int i = 0; c < 3 && i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            if (!spaceSeq)
                                substringStartIndices[c++] = i;
                            spaceSeq = true;
                        }
                        else
                            spaceSeq = false;
                    }
                    
                    // 1 - y since blender takes coords from lower left. While in the program we have considered an 2D array (of colors)/texture sarting from upper left.
                    float x = float.Parse(line.Substring(substringStartIndices[0] + 1, substringStartIndices[1] - substringStartIndices[0] - 1));
                    float y = 1f - float.Parse(line.Substring(substringStartIndices[1] + 1, (c == 3 ? substringStartIndices[2] : line.Length) - substringStartIndices[1] - 1));

                    uvs.AddLast(new Vector2D(x, y));
                }
                else if (line.StartsWith("f "))
                {
                    string vertexRegex = @" .*?\/";
                    Regex rx = new Regex(vertexRegex);
                    MatchCollection matches = rx.Matches(line);

                    for (int i = 0; i < 3; i++)
                    {
                        String str = matches[i].Value;
                        int vertexIndex = int.Parse(str.Substring(1, str.Length - 2)) - 1;
                        triangles.AddLast(vertexIndex);
                    }

                    if (!uvLoadingFailed)
                    {
                        string uvRegex = @"\d\/.*?\/";

                        rx = new Regex(uvRegex);

                        try
                        {
                            matches = rx.Matches(line);

                            for (int i = 0; i < 3; i++)
                            {
                                String str = matches[i].Value;
                                uvTriangles.AddLast(int.Parse(str.Substring(2, str.Length - 3)) - 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            uvLoadingFailed = true;

                            Log.InfoLine("Failed to load mesh UVs");
                        }
                    }

                    if (!normalLoadingFailed)
                    {
                        string normalRegex = @" \d*\/\d*\/\d*";
                        rx = new Regex(normalRegex);

                        try
                        {
                            matches = rx.Matches(line);

                            for (int i = 0; i < 3; i++)
                            {
                                String str = matches[i].Value;

                                normalTriangles.AddLast(int.Parse(str.Substring(str.LastIndexOf('/') + 1)) - 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            normalLoadingFailed = true;
                            Log.InfoLine("Failed to load mesh normals.");
                        }
                    }
                }
            }

            return new MeshBuilder() { vertices = vertices.ToArray<Vector3D>(), uvs = uvs.ToArray<Vector2D>(), uvTriangles = uvTriangles.ToArray<int>(), vertexTriangles = triangles.ToArray<int>(), normals = vertexNormals.ToArray<Vector3D>(), normalTriangles = normalTriangles.ToArray<int>()};
        }
    }
}