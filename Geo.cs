using System;
using System.Collections.Generic;
using System.Text;

namespace ViveBus
{
	class Geo
	{
		public struct Point
		{
			public double latitude;
			public double longitude;

			public Point(double _latitude, double _longitude)
			{
				latitude = _latitude;
				longitude = _longitude;
			}
		}

		public struct Place
		{
			public int id;
			public string name;
			public Point coordinates;
		}

		public struct Venue
		{
			public string id;
			public string name;
			public string searchable_name;
			public Point coordinates;
		}

		public struct Node
		{
			public int id;
			public int destination;
			public int weight;
		}

		public static double Distance(Point start, Point end)
		{
			double theta = start.longitude - end.longitude;
			double dist = Math.Sin(deg2rad(start.latitude)) * Math.Sin(deg2rad(end.latitude))
				+ Math.Cos(deg2rad(start.latitude)) * Math.Cos(deg2rad(end.latitude)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 111.1896;
			
			return (dist);
		}

		private static double deg2rad(double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		private static double rad2deg(double rad)
		{
			return (rad / Math.PI * 180.0);
		}

		public static string getDistanceString(double distance)
		{
			if (distance < 1)
			{
				// Convert to meters
				return String.Format("{0:0}", distance * 1000) + " metros";
			}
			else
			{
				return String.Format("{0:#.##}", distance) + " km";
			}
		}

		public static KeyValuePair<int, double> getLowestDistanceId(Point origin, List<Place> places)
		{
			KeyValuePair<int, double> lowest_distance = new KeyValuePair<int, double>(0, double.MaxValue);
			Dictionary<int, double> distances = new Dictionary<int, double>();
			foreach (Geo.Place place in places)
			{
				double current_distance = Geo.Distance(origin, place.coordinates);
				distances.Add(place.id, current_distance);

				if (current_distance < lowest_distance.Value)
				{
					lowest_distance = new KeyValuePair<int, double>(place.id, current_distance);
				}
			}

			return lowest_distance;
		}

		public static List<Place> loadPlacesFromCSV(string file_path)
		{
			List<Place> places = new List<Place>();
			
			string[] csv_lines = System.IO.File.ReadAllLines(file_path);
			int csv_lines_num = csv_lines.Length;
			if (csv_lines_num > 1)
			{
				for (int current_line = 1; current_line < csv_lines_num; ++current_line)
				{
					string[] parts = csv_lines[current_line].Split(',');

					Geo.Place place = new Geo.Place();
					int.TryParse(parts[0], out place.id);
					place.name = parts[1];

					double latitude;
					double longitude;
					double.TryParse(parts[2], out longitude);
					double.TryParse(parts[3], out latitude);
					place.coordinates = new Geo.Point(latitude, longitude);

					places.Add(place);
				}
			}

			return places;
		}

		public static List<Node> loadNodesFromCSV(string file_path)
		{
			List<Node> nodes = new List<Node>();

			string[] csv_lines = System.IO.File.ReadAllLines(file_path);
			int csv_lines_num = csv_lines.Length;
			if (csv_lines_num > 1)
			{
				for (int current_line = 1; current_line < csv_lines_num; ++current_line)
				{
					string[] parts = csv_lines[current_line].Split(',');

					Geo.Node node = new Geo.Node();
					int.TryParse(parts[0], out node.id);
					int.TryParse(parts[2], out node.destination);
					int.TryParse(parts[3], out node.weight);

					nodes.Add(node);
				}
			}

			return nodes;
		}

		public static List<Venue> loadVenuesFromCSV(string file_path)
		{
			List<Venue> venues = new List<Venue>();

			string[] csv_lines = System.IO.File.ReadAllLines(file_path);
			int csv_lines_num = csv_lines.Length;
			if (csv_lines_num > 1)
			{
				for (int current_line = 1; current_line < csv_lines_num; ++current_line)
				{
					string[] parts = csv_lines[current_line].Split('\t');

					Geo.Venue venue = new Geo.Venue();

					venue.id = parts[0];
					venue.name = parts[1];
					venue.searchable_name = parts[2];

					double latitude;
					double longitude;
					double.TryParse(parts[4], out longitude);
					double.TryParse(parts[3], out latitude);
					venue.coordinates = new Geo.Point(latitude, longitude);

					venues.Add(venue);
				}
			}

			return venues;
		}

		public static List<Node> getWeightFromDistances(List<Place> places, List<Node> nodes)
		{
			List<Node> nodes_with_distances = new List<Node>();
			foreach (Node current_node in nodes)
			{
				Node node_with_distance = new Node();
				int start = current_node.id;
				int end = current_node.destination;

				double weight = Distance(getPlaceFromId(start, places), getPlaceFromId(end, places));

				node_with_distance = current_node;
				node_with_distance.weight = (int)weight;

				nodes_with_distances.Add(node_with_distance);
			}

			return nodes_with_distances;
		}

		public static void saveNodesToCSV(List<Node> nodes, string file_path)
		{
			List<string> lines = new List<string>();

			foreach (Node current_node in nodes)
			{
				string line = current_node.id + "," + current_node.destination + "," + current_node.weight;
				lines.Add(line);
			}

			System.IO.File.WriteAllLines(file_path, lines.ToArray());
		}

		public static Point getPlaceFromId(int id, List<Place> places)
		{
			// TO DO: Use search instead

			foreach (Place current_place in places)
			{
				if (current_place.id == id)
				{
					return current_place.coordinates;
				}
			}

			return new Point();
		}

	}
}
