using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 日本麻将 {
	public class Player {
		internal YakuEnvironment environment;

		public YakuEnvironment Environment => environment;
		public Wind SelfWind { get; }

		public Player(Wind selfWind) {
			SelfWind = selfWind;
		}
	}
}
