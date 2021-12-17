using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RayTracing
{
    public class ObjParser
    {
        public static MeshBuilder ObjFileToMesh(String location)
        {
            LinkedList<Vector3D> vertices = new LinkedList<Vector3D>();
            LinkedList<Vector2D> uvs = new LinkedList<Vector2D>();

            LinkedList<int> triangles = new LinkedList<int>();
            LinkedList<int> uvTriangles = new LinkedList<int>();

            String[] lines = File.ReadAllLines(location);

            bool uvLoadingFailed = false;

            foreach (String line in lines)
            {
                if (line.StartsWith("v "))
                {
                    int[] substringStartIndices = new int[3];
                    int c = 0;

                    bool spaceSeq = false;

                    for (int i = 0; c < 3; i++)
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
                    float x = float.Parse(line.Substring(substringStartIndices[0] + 1, substringStartIndices[1] - substringStartIndices[0] - 1));
                    float y = float.Parse(line.Substring(substringStartIndices[1] + 1, substringStartIndices[2] - substringStartIndices[1] - 1));
                    float z = float.Parse(line.Substring(substringStartIndices[2] + 1));

                    vertices.AddLast(new Vector3D(x, y, z));
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

                    float x = float.Parse(line.Substring(substringStartIndices[0] + 1, substringStartIndices[1] - substringStartIndices[0] - 1));
                    float y = float.Parse(line.Substring(substringStartIndices[1] + 1, (c == 3 ? substringStartIndices[2] : line.Length) - substringStartIndices[1] - 1));

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

                    if (uvLoadingFailed)
                        continue;

                    string uvRegex = @"\d\/.*?\/";

                    rx = new Regex(uvRegex);
                    matches = rx.Matches(line);

                    try
                    {

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
            }

            return new MeshBuilder() { vertices = vertices.ToArray(), uvs = uvs.ToArray(), uvTriangles = uvTriangles.ToArray(), vertexTriangles = triangles.ToArray() };
        }
    }
}