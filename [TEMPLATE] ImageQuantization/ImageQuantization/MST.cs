﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class MST
    {
        static RGBPixel[,] Buffer = ImageOperations.GetImage(); //matrix that contains image pixels
        static int imageHeight = Buffer.GetLength(0);
        static int imageWidth = Buffer.GetLength(1);

        static int maxDistinctNum = imageHeight * imageWidth; //max no of distinct colors that can be found in matrix
        /*All Dictionaries initialized with maxDistinctNum as it is the max no of elements that can be added 
        to the Dictionary (Dictionary capacity) to avoid O(N) complexity when adding new elements*/
        //distinctColors > Dictionary that contains MinSpanningTree
        static Dictionary<string, KeyValuePair<string, double>> distinctColors = new Dictionary<string, KeyValuePair<string, double>>(maxDistinctNum);
        //distinctHelper > Helper Dictionary for keeping struct values concatenated as string (Key) to increase hashFunction operations
        static Dictionary<string, RGBPixel> distinctHelper = new Dictionary<string, RGBPixel>(maxDistinctNum);
        //visited > Dictionary to check visited nodes in MinSpnningTree
        static Dictionary<string, bool> visited = new Dictionary<string, bool>(maxDistinctNum);

        static int noDistinctColors = 0; //actual no of distinct colors, calculated in GetDistinctColors() function
        static double MST_Sum = 0;

        /// <summary>
        /// Gets all DistinctColors from 2D array and fills Dictionary of MST with all of them
        /// </summary>
        /// <returns>number of DistinctColors</returns>
        /// Time Complexity: O(N^2), N is the image width (or height)
        /// Space Complexity: N, N is the image width (or height)
        public static int FindDistinctColors()
        {
            //Outer for loop approaches an O(N) operation, N is the image height
            for (int i = 0; i < imageHeight; ++i)
            {
                //Inner for loop approaches an O(N) operation, N is the image width
                for (int j = 0; j < imageWidth; ++j)
                {
                    string color = //ToString() approaches an O(1) operation
                                  (Buffer[i, j].red).ToString()
                                  + (Buffer[i, j].green).ToString()
                                  + (Buffer[i, j].blue).ToString();
                    if (!distinctHelper.ContainsKey(color)) //ContainsKey() approaches an O(1) operation
                    {
                        RGBPixel node = Buffer[i, j]; //O(1)

                        //Add() approaches an O(1) operation if Count is less than the capacity,
                        //guaranteed to run in O(1) as map is initialized with max capacity
                        distinctColors.Add(color, new KeyValuePair<string, double>("", double.MaxValue));  //node initialized with max distance
                        distinctHelper.Add(color, node);
                        visited.Add(color, false); //node initially not visited

                        noDistinctColors++; //O(1)
                    }
                }
            }
            return noDistinctColors; //O(1)
        }

        /// <summary>
        /// Calculates distances between all vertices and each other, 
        /// and finds Minimum Spanning Tree using Prim's Algorithm
        /// </summary>
        /// <returns>Sum of MST</returns>
        /// Time Complexity: O(V^2) = O(E), V is the no of vertices, 
        /// E is the no of Edges in graph, approaches V^2 as it is a dense graph
        public static double FindMinimumSpanningTree()
        {
            //the minimum node by which we start calculating min from,
            //initialized with source vertix where we start MST from
            KeyValuePair<string, KeyValuePair<string, double>> minVertix = distinctColors.First(); //O(1)

            //currentMin >  to get the minimum vertix at each iteration
            KeyValuePair<string, KeyValuePair<string, double>> currentMin = new KeyValuePair<string, KeyValuePair<string, double>>();

            //Outer for loop approaches an O(V) operation, V is the number of vertices,
            //each iteration we relax a vertix
            for (int i = 0; i < distinctColors.Count - 1; i++)
            {
                visited[minVertix.Key] = true;
                double minEdge = double.MaxValue;

                //ToList() > Converts distinctColors Dictionary to List to be able to iterate over it,
                //approaches an O(V) operation, V is the number of vertices
                List<KeyValuePair<string, KeyValuePair<string, double>>> vertices = distinctColors.ToList();

                //Inner for loop approaches an O(V) operation, V is the number of vertices
                foreach (KeyValuePair<string, KeyValuePair<string, double>> node in vertices)
                {
                    //check whether the distance must be calculated or not
                    //continue if node is already visited or it is the same node where the min distance is calc relative to it
                    if (((distinctHelper[node.Key].red == distinctHelper[minVertix.Key].red) &&
                        (distinctHelper[node.Key].blue == distinctHelper[minVertix.Key].red) &&
                        (distinctHelper[node.Key].green == distinctHelper[minVertix.Key].green))
                        || visited[node.Key] == true)
                    {
                        continue;
                    }
                    //Pow() approaches O(1) operation as O(log2(2)) = 1, Sqrt() approaches O(1) operation
                    double distance = Math.Sqrt(Math.Pow((distinctHelper[minVertix.Key].red - distinctHelper[node.Key].red), 2)
                                              + Math.Pow((distinctHelper[minVertix.Key].blue - distinctHelper[node.Key].blue), 2)
                                              + Math.Pow((distinctHelper[minVertix.Key].green - distinctHelper[node.Key].green), 2));
                    //update distance value if less distance is calculated
                    if (distinctColors[node.Key].Value > distance)
                    {
                        distinctColors[node.Key] = new KeyValuePair<string, double>(minVertix.Key, distance);
                    }
                    //finding cureent min in inner loop relative to chosen vertex in outer loop
                    if (distinctColors[node.Key].Value < minEdge)
                    {
                        minEdge = distinctColors[node.Key].Value;
                        currentMin = node;
                    }
                }
                minVertix = currentMin;
                MST_Sum += minEdge;
            }
            return MST_Sum; //O(1)
        }
        public static Dictionary<string, KeyValuePair<string, double>> GetMST()
        {
            return distinctColors;
        }
        public static Dictionary<string, RGBPixel> GetMSTHelper()
        {
            return distinctHelper;
        }
    }
}