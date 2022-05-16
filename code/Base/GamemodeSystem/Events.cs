namespace Sports;

public partial class Events
{
	public partial class Server
	{
	}

	public partial class Shared
	{
	}

	public partial class Client
	{
		public const string LocalGamemodeChanged = "Client.LocalGamemodeChanged";
		public const string ClientGamemodeChanged = "Client.ClientGamemodeChanged";

		public class LocalGamemodeChangedAttribute : EventAttribute
		{
			public LocalGamemodeChangedAttribute() : base( LocalGamemodeChanged ) { }
		}

		public class ClientGamemodeChangedAttribute : EventAttribute
		{
			public ClientGamemodeChangedAttribute() : base( ClientGamemodeChanged ) { }
		}
	}
}
