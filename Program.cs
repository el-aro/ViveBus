/**
 * GeekNights CUU - App for ViveBus
 * Module by @eaplmx
 * 
 * TO DO:
 * - Select when you have to get out of the Bus route to change path
 * - Change the strings to translatable table
 * - Fuzzy search in venues names
 * - Organize the code (Don't repeat yourself!)
 * - Add licences and credits for the code
 */

using System;
using System.Collections.Generic;
using System.Text;

using Dijkstras_Algorithm;

namespace ViveBus
{
	/// <summary>
	/// Sample code to find public transport routes between a start and end points
	/// Ask for two places, and get the bus stations to take between them
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{	
			// Load all our data from CSV files
			string executable_path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			string places_file_path = executable_path + System.IO.Path.DirectorySeparatorChar + "stations.csv";
			List<Geo.Station> places = Geo.loadPlacesFromCSV(places_file_path);

			string nodes_file_path = executable_path + System.IO.Path.DirectorySeparatorChar + "nodes.csv";
			List<Geo.Node> nodes = Geo.loadNodesFromCSV(nodes_file_path);

			string venues_file_path = executable_path + System.IO.Path.DirectorySeparatorChar + "venues.csv";
			List<Geo.Venue> venues = Geo.loadVenuesFromCSV(venues_file_path);

			Console.WriteLine("Bienvenido a ViveBus app");
			Console.WriteLine("------------------------");
			Console.WriteLine("Introduce un lugar público cerca de donde estás en este momento ");
			Console.WriteLine("y un lugar cerca de tu destino");
			Console.WriteLine();
			Console.WriteLine("Te mostraré la ruta de ViveBus más cercana");
			Console.WriteLine();
			Console.WriteLine("Para más ayuda has click en el ícono de ayuda (si lo encuentras)");
			Console.WriteLine("------------------------");

			// Save the distances between each place, to set the weight for the edges
			// Only run this once to generate the table
			/*
			file_path = executable_paht + System.IO.Path.DirectorySeparatorChar + "nodes_distances.csv";
			Geo.saveNodesToCSV(Geo.getWeightFromDistances(places, nodes), file_path);
			 * */

			double distance;

			// DEBUG: Origin and destination points for tests
			//Geo.Point origin = new Geo.Point(28.664536, -106.109754);
			//Geo.Point destination = new Geo.Point(28.634536, -106.102754);

			string station_name = string.Empty;

			Console.WriteLine("");
			Console.WriteLine("¿Dónde estás?");

			bool found_venues = false;
			List<Geo.Venue> origin_venues = new List<Geo.Venue>();

			while (found_venues == false)
			{
				// Get input from the user, which is your selection?
				string origin_venue = Console.ReadLine();
				origin_venues = searchVenues(origin_venue, venues);

				if (origin_venues.Count == 0)
				{
					Console.WriteLine("No encontré lugares con ese nombre, intenta otro");
				}
				else
				{
					Console.WriteLine("Encontré los siguientes lugares, selecciona uno:");
					Console.WriteLine("Escribe el número");
					found_venues = true;
				}
			}

			int current_venue_num = 1; // The list starts with 1
			foreach (Geo.Venue current_venue in origin_venues)
			{
				Console.WriteLine(current_venue_num + " - " + current_venue.name);
				++current_venue_num;
			}

			Geo.Venue origin_venue_selected = new Geo.Venue();
			bool invalid_selection = true;
			while (invalid_selection)
			{
				string selected_origin_string = Console.ReadLine();
				int selected_origin;
				int.TryParse(selected_origin_string, out selected_origin);
				if (selected_origin > 0 && (selected_origin - 1) < origin_venues.Count)
				{
					origin_venue_selected = origin_venues[selected_origin - 1];
					invalid_selection = false;
				}
				else
				{
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					Console.WriteLine();
					Console.SetCursorPosition(0, Console.CursorTop - 1);
				}
			}

			Geo.Point origin_point = origin_venue_selected.coordinates;

			Console.WriteLine();
			Console.WriteLine("¿A dónde quieres llegar?");

			// DRY - F***!!! Fix this
			// This is a copy-paste from the code above
			found_venues = false;
			while (found_venues == false)
			{
				// Get input from the user, which is your selection?
				string origin_venue = Console.ReadLine();
				origin_venues = searchVenues(origin_venue, venues);

				if (origin_venues.Count == 0)
				{
					Console.WriteLine("No encontré lugares con ese nombre, intenta otro");
				}
				else
				{
					Console.WriteLine("Encontré los siguientes lugares, selecciona uno:");
					Console.WriteLine("Escribe el número");
					found_venues = true;
				}
			}

			current_venue_num = 1; // The list starts with 1
			foreach (Geo.Venue current_venue in origin_venues)
			{
				Console.WriteLine(current_venue_num + " - " + current_venue.name);
				++current_venue_num;
			}

			origin_venue_selected = new Geo.Venue();
			invalid_selection = true;
			while (invalid_selection)
			{
				string selected_origin_string = Console.ReadLine();
				int selected_origin;
				int.TryParse(selected_origin_string, out selected_origin);
				if (selected_origin > 0 && (selected_origin - 1) < origin_venues.Count)
				{
					origin_venue_selected = origin_venues[selected_origin - 1];
					invalid_selection = false;
				}
				else
				{
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					Console.WriteLine();
					Console.SetCursorPosition(0, Console.CursorTop - 1);
				}
			}

			Geo.Point destination = origin_venue_selected.coordinates;

			Console.WriteLine("---------------------");

			// Get the distances to all the stations
			KeyValuePair<int, double> lowest_distance = Geo.getLowestDistanceId(origin_point, places);
			int origin_id = lowest_distance.Key;

			// Find the station closer to this destination
			foreach (Geo.Station place in places)
			{
				if (place.id == lowest_distance.Key)
				{
					station_name = place.name;
				}
			}

			Console.WriteLine("Ve en alfombra voladora ");
			distance = lowest_distance.Value;
			Console.WriteLine(Geo.getDistanceString(distance));
			Console.WriteLine("hasta ");
			Console.WriteLine(station_name);

			station_name = string.Empty;
			lowest_distance = Geo.getLowestDistanceId(destination, places);

			int destination_id = lowest_distance.Key;

			foreach (Geo.Station place in places)
			{
				if (place.id == lowest_distance.Key)
				{
					station_name = place.name;
				}
			}

			Console.WriteLine();
			Console.WriteLine("baja en ");
			Console.WriteLine(station_name);
			Console.WriteLine();
			Console.WriteLine("y teletranspórtate ");

			distance = lowest_distance.Value;
			Console.WriteLine(Geo.getDistanceString(distance));

			Console.WriteLine("hasta tu destino ");

			// Now we'll get the shortest path between stations
			Graph graph = new Graph();
			Dictionary<int, Vector2D> resultIndexVertexMapping = new Dictionary<int, Vector2D>();

			foreach (Geo.Station current_place in places)
			{
				graph.AddVertex(new Vector2D(0, 0, false, current_place.id));
			}

			List<Vector2D> graph_nodes = graph.AllNodes;

			foreach (Geo.Node current_node in nodes)
			{
				graph.AddEdge(new Edge(getVectorFromId(current_node.id, graph_nodes),
					getVectorFromId(current_node.destination, graph_nodes), current_node.weight));
			}

			graph.SourceVertex = getVectorFromId(origin_id, graph_nodes);

			bool reachable = graph.CalculateShortestPath();
			List<Vector2D> path = graph.RetrieveShortestPath(getVectorFromId(destination_id, graph_nodes));

			double cost = path[path.Count - 1].AggregateCost;

			Console.WriteLine("---------------------");
			Console.WriteLine("Ahora te daré todas las conexiones intermedias:");

			foreach (Vector2D current_vector in path)
			{
				foreach (Geo.Station place in places)
				{
					if (place.id == current_vector.VertexID)
					{
						station_name = place.name;
						Console.WriteLine(station_name);
					}
				}
			}

			/*
			string sNew = "Vive Bus";
			string sOld = "vivebus";

			try
			{
				Levenshtein l = new Levenshtein();

				int counter = 0;
				while (counter < 1)
				{
					DateTime startTime = DateTime.Now;
					DateTime stopTime = DateTime.Now;
					TimeSpan duration = stopTime - startTime;

					startTime = DateTime.Now;

					l.iLD(sNew, sOld);

					stopTime = DateTime.Now;

					duration = stopTime - startTime;
					Console.WriteLine("New     :" + duration);
					Console.WriteLine("----------------");

					++counter;
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.ToString());
			}
			 */

			Console.WriteLine();
			Console.Write("Cualquier tecla para salir");
			Console.ReadKey();
		}

		// TO DO: Move this to the controller
		public static Vector2D getVectorFromId(int id, List<Vector2D> nodes)
		{
			foreach (Vector2D current_node in nodes)
			{
				if (current_node.VertexID == id)
				{
					return current_node;
				}
			}

			// TO DO: Correct this exception case
			return new Vector2D(0, 0, false, 0);
		}

		/// <summary>
		/// Get a list of venues, matching the name
		/// TO DO:
		/// - Fuzzy search when we get no results. (For example, if you search Four Square, you're not
		/// going to find FourSquare
		/// </summary>
		/// <param name="search_string">Name of the venue to find</param>
		/// <param name="venues">List of venues to </param>
		/// <returns>List of venues where the search string is present</returns>
		public static List<Geo.Venue> searchVenues(string search_string, List<Geo.Venue> venues)
		{
			List<Geo.Venue> found_venues = new List<Geo.Venue>();
			search_string = replaceLatinCharacters(search_string.ToLower());

			string[] search_words = search_string.Split(' ');

			Dictionary<string, int> search_votes = new Dictionary<string, int>();

			foreach (string current_word in search_words)
			{
				foreach (Geo.Venue current_venue in venues)
				{
					string venue_name = replaceLatinCharacters(current_venue.name).ToLower();
					if (venue_name.Contains(current_word))
					{
						// Add votes each time you find a word
						if (search_votes.ContainsKey(current_venue.id))
						{
							++search_votes[current_venue.id];
						}
						else
						{
							search_votes[current_venue.id] = 1;
						}
					}
				}
			}


			foreach (KeyValuePair<string, int> vote in search_votes)
			{
				if (vote.Value > 0)
				{
					found_venues.Add(findVenueByID(vote.Key, venues));
				}
			}

			return found_venues;
		}

		/// <summary>
		/// From a list of Venues, get that venue matching with the id supplied
		/// </summary>
		/// <param name="id">String with the ID of the venue to search</param>
		/// <param name="venues">List of venues to search</param>
		/// <returns>Venue</returns>
		public static Geo.Venue findVenueByID(string id, List<Geo.Venue> venues)
		{
			foreach (Geo.Venue current_venue in venues)
			{
				if (current_venue.id == id)
				{
					return current_venue;
				}
			}

			return new Geo.Venue();
		}

		/// <summary>
		/// Remove accents and tilde from the character
		/// </summary>
		/// <param name="string_to_replace"></param>
		/// <returns>String with the latin characters removed</returns>
		public static string replaceLatinCharacters(string string_to_replace)
		{
			string_to_replace = string_to_replace.Replace('á', 'a');
			string_to_replace = string_to_replace.Replace('é', 'e');
			string_to_replace = string_to_replace.Replace('í', 'i');
			string_to_replace = string_to_replace.Replace('ó', 'o');
			string_to_replace = string_to_replace.Replace('ú', 'u');
			string_to_replace = string_to_replace.Replace('Á', 'A');
			string_to_replace = string_to_replace.Replace('É', 'E');
			string_to_replace = string_to_replace.Replace('Í', 'I');
			string_to_replace = string_to_replace.Replace('Ó', 'O');
			string_to_replace = string_to_replace.Replace('Ú', 'U');
			string_to_replace = string_to_replace.Replace('Ú', 'U');
			string_to_replace = string_to_replace.Replace('ü', 'u');
			string_to_replace = string_to_replace.Replace('Ü', 'U');
			string_to_replace = string_to_replace.Replace('ñ', 'n');
			string_to_replace = string_to_replace.Replace('Ñ', 'N');

			return string_to_replace;
		}

	}

	/// <summary>
	/// Code for fuzzy search (Not implemented yet)
	/// </summary>
	public class Levenshtein
	{
		///*****************************
		/// Compute Levenshtein distance 
		/// Memory efficient version
		///*****************************
		public int iLD(String sRow, String sCol)
		{
			int RowLen = sRow.Length;  // length of sRow
			int ColLen = sCol.Length;  // length of sCol
			int RowIdx;                // iterates through sRow
			int ColIdx;                // iterates through sCol
			char Row_i;                // ith character of sRow
			char Col_j;                // jth character of sCol
			int cost;                  // cost

			/// Test string length
			if (Math.Max(sRow.Length, sCol.Length) > Math.Pow(2, 31))
				throw (new Exception("\nMaximum string length in Levenshtein.iLD is " + Math.Pow(2, 31) + ".\nYours is " + Math.Max(sRow.Length, sCol.Length) + "."));

			// Step 1
			if (RowLen == 0)
			{
				return ColLen;
			}

			if (ColLen == 0)
			{
				return RowLen;
			}

			/// Create the two vectors
			int[] v0 = new int[RowLen + 1];
			int[] v1 = new int[RowLen + 1];
			int[] vTmp;

			/// Step 2
			/// Initialize the first vector
			for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
			{
				v0[RowIdx] = RowIdx;
			}

			// Step 3
			/// For each column
			for (ColIdx = 1; ColIdx <= ColLen; ColIdx++)
			{
				/// Set the 0'th element to the column number
				v1[0] = ColIdx;

				Col_j = sCol[ColIdx - 1];


				// Step 4
				/// Fore each row
				for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
				{
					Row_i = sRow[RowIdx - 1];


					// Step 5
					if (Row_i == Col_j)
					{
						cost = 0;
					}
					else
					{
						cost = 1;
					}

					// Step 6
					/// Find minimum
					int m_min = v0[RowIdx] + 1;
					int b = v1[RowIdx - 1] + 1;
					int c = v0[RowIdx - 1] + cost;

					if (b < m_min)
					{
						m_min = b;
					}
					if (c < m_min)
					{
						m_min = c;
					}

					v1[RowIdx] = m_min;
				}

				/// Swap the vectors
				vTmp = v0;
				v0 = v1;
				v1 = vTmp;

			}


			// Step 7

			/// Value between 0 - 100
			/// 0==perfect match 100==totaly different
			/// 
			/// The vectors where swaped one last time at the end of the last loop,
			/// that is why the result is now in v0 rather than in v1
			System.Console.WriteLine("iDist=" + v0[RowLen]);
			int max = System.Math.Max(RowLen, ColLen);
			return ((100 * v0[RowLen]) / max);
		}

		///*****************************
		/// Compute the min
		///*****************************
		private int Minimum(int a, int b, int c)
		{
			int mi = a;

			if (b < mi)
			{
				mi = b;
			}
			if (c < mi)
			{
				mi = c;
			}

			return mi;
		}

		///*****************************
		/// Compute Levenshtein distance         
		///*****************************
		public int LD(String sNew, String sOld)
		{
			int[,] matrix;              // matrix
			int sNewLen = sNew.Length;  // length of sNew
			int sOldLen = sOld.Length;  // length of sOld
			int sNewIdx;					// iterates through sNew
			int sOldIdx;					// iterates through sOld
			char sNew_i;					// ith character of sNew
			char sOld_j;					// jth character of sOld
			int cost;					// cost

			/// Test string length
			if (Math.Max(sNew.Length, sOld.Length) > Math.Pow(2, 31))
				throw (new Exception("\nMaximum string length in Levenshtein.LD is " + Math.Pow(2, 31) + ".\nYours is " + Math.Max(sNew.Length, sOld.Length) + "."));

			// Step 1
			if (sNewLen == 0)
			{
				return sOldLen;
			}

			if (sOldLen == 0)
			{
				return sNewLen;
			}

			matrix = new int[sNewLen + 1, sOldLen + 1];

			// Step 2
			for (sNewIdx = 0; sNewIdx <= sNewLen; sNewIdx++)
			{
				matrix[sNewIdx, 0] = sNewIdx;
			}

			for (sOldIdx = 0; sOldIdx <= sOldLen; sOldIdx++)
			{
				matrix[0, sOldIdx] = sOldIdx;
			}

			// Step 3
			for (sNewIdx = 1; sNewIdx <= sNewLen; sNewIdx++)
			{
				sNew_i = sNew[sNewIdx - 1];

				// Step 4
				for (sOldIdx = 1; sOldIdx <= sOldLen; sOldIdx++)
				{
					sOld_j = sOld[sOldIdx - 1];

					// Step 5
					if (sNew_i == sOld_j)
					{
						cost = 0;
					}
					else
					{
						cost = 1;
					}

					// Step 6
					matrix[sNewIdx, sOldIdx] = Minimum(matrix[sNewIdx - 1, sOldIdx] + 1, matrix[sNewIdx, sOldIdx - 1] + 1, matrix[sNewIdx - 1, sOldIdx - 1] + cost);
				}
			}

			// Step 7

			/// Value between 0 - 100
			/// 0==perfect match 100==totaly different
			System.Console.WriteLine("Dist=" + matrix[sNewLen, sOldLen]);
			int max = System.Math.Max(sNewLen, sOldLen);
			return (100 * matrix[sNewLen, sOldLen]) / max;
		}
	}

}