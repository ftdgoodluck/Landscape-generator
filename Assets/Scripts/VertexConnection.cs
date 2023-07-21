using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace mattatz.MeshSmoothingSystem {

	public class VertexConnection {

		public List<int> Connection { get { return connection; } }

		List<int> connection;

		public VertexConnection() {
			this.connection = new List<int>();
		}

		public void Connect (int to) {
			connection.Add(to);
		}

		public static Dictionary<int, VertexConnection> BuildNetwork (List<int> triangles) {
			var table = new Dictionary<int, VertexConnection>();

			for(int i = 0, n = triangles.Count; i < n; i += 3) {
				int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
				if(!table.ContainsKey(a)) {
					table.Add(a, new VertexConnection());
				}
				if(!table.ContainsKey(b)) {
					table.Add(b, new VertexConnection());
				}
				if(!table.ContainsKey(c)) {
					table.Add(c, new VertexConnection());
				}
				if (!table[a].connection.Contains(b))
                    table[a].Connect(b);
                if (!table[a].connection.Contains(c))
                    table[a].Connect(c);
                if (!table[b].connection.Contains(a))
                    table[b].Connect(a);
                if (!table[b].connection.Contains(c))
                    table[b].Connect(c);
                if (!table[c].connection.Contains(a))
                    table[c].Connect(a);
                if (!table[c].connection.Contains(b))
                    table[c].Connect(b);
                
				 
			}

			return table;
		}

	}

}

