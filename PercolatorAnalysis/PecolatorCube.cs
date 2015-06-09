
/**********************************************************************************************************************
*
*								Thank you for using Percolator Analysis Services!!!!!!!
*
*	WARNING: Changing this auto generated file may lead to wonky actions, and could possibly injure many puppies. 
*			 Any manual changes will be lost upon new generation. 
*
*	CONNECTION STRING: Provider=MSOLAP;Data Source=SQL01;Initial Catalog=ReSight Cube;Cube=ReSight;User ID=coopdigity\ReportsUser; Password=$h0wM3th3Rep0rts!
*	LAST GENERATION: 12/11/2014 14:30:59
*
**********************************************************************************************************************/
using System.Configuration;

namespace PercolatorAnalysis.Model
{
	using PercolatorAnalysis.Base;
	using PercolatorAnalysis.Attributes;
	using PercolatorAnalysis.Linq;


	public sealed class ElCubo : CubeBase
	{
		static string _CON { get { return ConfigurationManager.ConnectionStrings["ReSight"].ConnectionString; } }

		public ElCubo()
			: base(_CON) { }
		
		#region Cubes
		public Cube<ReSight> ReSight { get { return new Cube<ReSight>(this._provider); } }
		#endregion
	}

	#region ReSight Dimensions
	[Dimension]
	public sealed class ReSight_Calendar_Dimension : ICubeObject
	{
		[Tag("[Calendar].[Month Abbrev]")]
		public Attribute MonthAbbrev { get { return new Attribute("[Calendar].[Month Abbrev]"); } }
		[Tag("[Calendar].[Month End]")]
		public Attribute MonthEnd { get { return new Attribute("[Calendar].[Month End]"); } }
		[Tag("[Calendar].[Month ID]")]
		public Attribute MonthID { get { return new Attribute("[Calendar].[Month ID]"); } }
		[Tag("[Calendar].[Month Label]")]
		public Attribute MonthLabel { get { return new Attribute("[Calendar].[Month Label]"); } }
		[Tag("[Calendar].[Month Name]")]
		public Attribute MonthName { get { return new Attribute("[Calendar].[Month Name]"); } }
		[Tag("[Calendar].[Month Number]")]
		public Attribute MonthNumber { get { return new Attribute("[Calendar].[Month Number]"); } }
		[Tag("[Calendar].[Month Start]")]
		public Attribute MonthStart { get { return new Attribute("[Calendar].[Month Start]"); } }
		[Tag("[Calendar].[Quarter Abbrev]")]
		public Attribute QuarterAbbrev { get { return new Attribute("[Calendar].[Quarter Abbrev]"); } }
		[Tag("[Calendar].[Quarter End]")]
		public Attribute QuarterEnd { get { return new Attribute("[Calendar].[Quarter End]"); } }
		[Tag("[Calendar].[Quarter ID]")]
		public Attribute QuarterID { get { return new Attribute("[Calendar].[Quarter ID]"); } }
		[Tag("[Calendar].[Quarter Name]")]
		public Attribute QuarterName { get { return new Attribute("[Calendar].[Quarter Name]"); } }
		[Tag("[Calendar].[Quarter Start]")]
		public Attribute QuarterStart { get { return new Attribute("[Calendar].[Quarter Start]"); } }
		[Tag("[Calendar].[Week Abbrev]")]
		public Attribute WeekAbbrev { get { return new Attribute("[Calendar].[Week Abbrev]"); } }
		[Tag("[Calendar].[Week End]")]
		public Attribute WeekEnd { get { return new Attribute("[Calendar].[Week End]"); } }
		[Tag("[Calendar].[Week ID]")]
		public Attribute WeekID { get { return new Attribute("[Calendar].[Week ID]"); } }
		[Tag("[Calendar].[Week Name]")]
		public Attribute WeekName { get { return new Attribute("[Calendar].[Week Name]"); } }
		[Tag("[Calendar].[Week Start]")]
		public Attribute WeekStart { get { return new Attribute("[Calendar].[Week Start]"); } }
		[Tag("[Calendar].[Year Abbrev]")]
		public Attribute YearAbbrev { get { return new Attribute("[Calendar].[Year Abbrev]"); } }
		[Tag("[Calendar].[Year End]")]
		public Attribute YearEnd { get { return new Attribute("[Calendar].[Year End]"); } }
		[Tag("[Calendar].[Year ID]")]
		public Attribute YearID { get { return new Attribute("[Calendar].[Year ID]"); } }
		[Tag("[Calendar].[Year Name]")]
		public Attribute YearName { get { return new Attribute("[Calendar].[Year Name]"); } }
		[Tag("[Calendar].[Year Start]")]
		public Attribute YearStart { get { return new Attribute("[Calendar].[Year Start]"); } }
		
		[Hierarchy]
		public CalendarbyWeek_Hierarchy CalendarbyWeek { get { return new CalendarbyWeek_Hierarchy(); } }
		
		public sealed class CalendarbyWeek_Hierarchy : Hierarchy, ICubeObject
		{
			public CalendarbyWeek_Hierarchy()
				: base("[Calendar].[Calendar by Week]") { }

			[Tag("[Calendar].[Calendar by Week].[Year]", 1)]
			public Level Year { get { return new Level("[Calendar].[Calendar by Week].[Year]", 1); } }
			[Tag("[Calendar].[Calendar by Week].[Quarter]", 2)]
			public Level Quarter { get { return new Level("[Calendar].[Calendar by Week].[Quarter]", 2); } }
			[Tag("[Calendar].[Calendar by Week].[Month]", 3)]
			public Level Month { get { return new Level("[Calendar].[Calendar by Week].[Month]", 3); } }
			[Tag("[Calendar].[Calendar by Week].[Week]", 4)]
			public Level Week { get { return new Level("[Calendar].[Calendar by Week].[Week]", 4); } }
		}
	}

	[Dimension]
	public sealed class ReSight_ChainCluster_Dimension : ICubeObject
	{
		[Tag("[Chain Cluster].[Chain ID]")]
		public Attribute ChainID { get { return new Attribute("[Chain Cluster].[Chain ID]"); } }
		[Tag("[Chain Cluster].[ChainClustKey]")]
		public Attribute ChainClustKey { get { return new Attribute("[Chain Cluster].[ChainClustKey]"); } }
		[Tag("[Chain Cluster].[Login]")]
		public Attribute Login { get { return new Attribute("[Chain Cluster].[Login]"); } }
	}

	[Dimension]
	public sealed class ReSight_DistCluster_Dimension : ICubeObject
	{
		[Tag("[Dist Cluster].[Dist ID]")]
		public Attribute DistID { get { return new Attribute("[Dist Cluster].[Dist ID]"); } }
		[Tag("[Dist Cluster].[DistClustKey]")]
		public Attribute DistClustKey { get { return new Attribute("[Dist Cluster].[DistClustKey]"); } }
		[Tag("[Dist Cluster].[Login]")]
		public Attribute Login { get { return new Attribute("[Dist Cluster].[Login]"); } }
	}

	[Dimension]
	public sealed class ReSight_DistributorDCs_Dimension : ICubeObject
	{
		[Tag("[Distributor DCs].[DC Label]")]
		public Attribute DCLabel { get { return new Attribute("[Distributor DCs].[DC Label]"); } }
		[Tag("[Distributor DCs].[DC Location]")]
		public Attribute DCLocation { get { return new Attribute("[Distributor DCs].[DC Location]"); } }
		[Tag("[Distributor DCs].[DC Location ID]")]
		public Attribute DCLocationID { get { return new Attribute("[Distributor DCs].[DC Location ID]"); } }
		[Tag("[Distributor DCs].[DC Location Name]")]
		public Attribute DCLocationName { get { return new Attribute("[Distributor DCs].[DC Location Name]"); } }
		[Tag("[Distributor DCs].[DCID]")]
		public Attribute DCID { get { return new Attribute("[Distributor DCs].[DCID]"); } }
		[Tag("[Distributor DCs].[Dist ID]")]
		public Attribute DistID { get { return new Attribute("[Distributor DCs].[Dist ID]"); } }
		[Tag("[Distributor DCs].[Dist Name]")]
		public Attribute DistName { get { return new Attribute("[Distributor DCs].[Dist Name]"); } }
		[Tag("[Distributor DCs].[DistDC]")]
		public Attribute DistDC { get { return new Attribute("[Distributor DCs].[DistDC]"); } }
	}

	[Dimension]
	public sealed class ReSight_Distributors_Dimension : ICubeObject
	{
		[Tag("[Distributors].[Dist ID]")]
		public Attribute DistID { get { return new Attribute("[Distributors].[Dist ID]"); } }
		[Tag("[Distributors].[Dist Name]")]
		public Attribute DistName { get { return new Attribute("[Distributors].[Dist Name]"); } }
		[Tag("[Distributors].[Dollars Extended]")]
		public Attribute DollarsExtended { get { return new Attribute("[Distributors].[Dollars Extended]"); } }
		[Tag("[Distributors].[Inventory Script]")]
		public Attribute InventoryScript { get { return new Attribute("[Distributors].[Inventory Script]"); } }
		[Tag("[Distributors].[Location Script]")]
		public Attribute LocationScript { get { return new Attribute("[Distributors].[Location Script]"); } }
		[Tag("[Distributors].[Sales Script]")]
		public Attribute SalesScript { get { return new Attribute("[Distributors].[Sales Script]"); } }
		[Tag("[Distributors].[Weight Extended]")]
		public Attribute WeightExtended { get { return new Attribute("[Distributors].[Weight Extended]"); } }
	}

	[Dimension]
	public sealed class ReSight_DSMCluster_Dimension : ICubeObject
	{
		[Tag("[DSM Cluster].[DSMID]")]
		public Attribute DSMID { get { return new Attribute("[DSM Cluster].[DSMID]"); } }
		[Tag("[DSM Cluster].[DSMKey]")]
		public Attribute DSMKey { get { return new Attribute("[DSM Cluster].[DSMKey]"); } }
		[Tag("[DSM Cluster].[Login]")]
		public Attribute Login { get { return new Attribute("[DSM Cluster].[Login]"); } }
	}

	[Dimension]
	public sealed class ReSight_Items_Dimension : ICubeObject
	{
		[Tag("[Items].[Brand]")]
		public Attribute Brand { get { return new Attribute("[Items].[Brand]"); } }
		[Tag("[Items].[Code]")]
		public Attribute Code { get { return new Attribute("[Items].[Code]"); } }
		[Tag("[Items].[Default Pack Type]")]
		public Attribute DefaultPackType { get { return new Attribute("[Items].[Default Pack Type]"); } }
		[Tag("[Items].[Depth]")]
		public Attribute Depth { get { return new Attribute("[Items].[Depth]"); } }
		[Tag("[Items].[Each Count]")]
		public Attribute EachCount { get { return new Attribute("[Items].[Each Count]"); } }
		[Tag("[Items].[Height]")]
		public Attribute Height { get { return new Attribute("[Items].[Height]"); } }
		[Tag("[Items].[Item ID]")]
		public Attribute ItemID { get { return new Attribute("[Items].[Item ID]"); } }
		[Tag("[Items].[Label Weight]")]
		public Attribute LabelWeight { get { return new Attribute("[Items].[Label Weight]"); } }
		[Tag("[Items].[Label Weight Unit]")]
		public Attribute LabelWeightUnit { get { return new Attribute("[Items].[Label Weight Unit]"); } }
		[Tag("[Items].[Life Stage]")]
		public Attribute LifeStage { get { return new Attribute("[Items].[Life Stage]"); } }
		[Tag("[Items].[Long Item Name]")]
		public Attribute LongItemName { get { return new Attribute("[Items].[Long Item Name]"); } }
		[Tag("[Items].[Manufacturer]")]
		public Attribute Manufacturer { get { return new Attribute("[Items].[Manufacturer]"); } }
		[Tag("[Items].[MAP]")]
		public Attribute MAP { get { return new Attribute("[Items].[MAP]"); } }
		[Tag("[Items].[NFKP]")]
		public Attribute NFKP { get { return new Attribute("[Items].[NFKP]"); } }
		[Tag("[Items].[Other Item Name]")]
		public Attribute OtherItemName { get { return new Attribute("[Items].[Other Item Name]"); } }
		[Tag("[Items].[Pack Type]")]
		public Attribute PackType { get { return new Attribute("[Items].[Pack Type]"); } }
		[Tag("[Items].[Short Item Name]")]
		public Attribute ShortItemName { get { return new Attribute("[Items].[Short Item Name]"); } }
		[Tag("[Items].[Size Group]")]
		public Attribute SizeGroup { get { return new Attribute("[Items].[Size Group]"); } }
		[Tag("[Items].[Species]")]
		public Attribute Species { get { return new Attribute("[Items].[Species]"); } }
		[Tag("[Items].[SRP]")]
		public Attribute SRP { get { return new Attribute("[Items].[SRP]"); } }
		[Tag("[Items].[Status]")]
		public Attribute Status { get { return new Attribute("[Items].[Status]"); } }
		[Tag("[Items].[Sub Brand]")]
		public Attribute SubBrand { get { return new Attribute("[Items].[Sub Brand]"); } }
		[Tag("[Items].[SWP]")]
		public Attribute SWP { get { return new Attribute("[Items].[SWP]"); } }
		[Tag("[Items].[Type]")]
		public Attribute Type { get { return new Attribute("[Items].[Type]"); } }
		[Tag("[Items].[UPC]")]
		public Attribute UPC { get { return new Attribute("[Items].[UPC]"); } }
		[Tag("[Items].[Weight]")]
		public Attribute Weight { get { return new Attribute("[Items].[Weight]"); } }
		[Tag("[Items].[Width]")]
		public Attribute Width { get { return new Attribute("[Items].[Width]"); } }
	}

	[Dimension]
	public sealed class ReSight_Locations_Dimension : ICubeObject
	{
		[Tag("[Locations].[Chain ID]")]
		public Attribute ChainID { get { return new Attribute("[Locations].[Chain ID]"); } }
		[Tag("[Locations].[Chain Location ID]")]
		public Attribute ChainLocationID { get { return new Attribute("[Locations].[Chain Location ID]"); } }
		[Tag("[Locations].[Channel]")]
		public Attribute Channel { get { return new Attribute("[Locations].[Channel]"); } }
		[Tag("[Locations].[Creation Date]")]
		public Attribute CreationDate { get { return new Attribute("[Locations].[Creation Date]"); } }
		[Tag("[Locations].[DSM ID]")]
		public Attribute DSMID { get { return new Attribute("[Locations].[DSM ID]"); } }
		[Tag("[Locations].[DSM Name]")]
		public Attribute DSMName { get { return new Attribute("[Locations].[DSM Name]"); } }
		[Tag("[Locations].[DSM Territory]")]
		public Attribute DSMTerritory { get { return new Attribute("[Locations].[DSM Territory]"); } }
		[Tag("[Locations].[Franchise]")]
		public Attribute Franchise { get { return new Attribute("[Locations].[Franchise]"); } }
		[Tag("[Locations].[Latitude]")]
		public Attribute Latitude { get { return new Attribute("[Locations].[Latitude]"); } }
		[Tag("[Locations].[Location City]")]
		public Attribute LocationCity { get { return new Attribute("[Locations].[Location City]"); } }
		[Tag("[Locations].[Location Country]")]
		public Attribute LocationCountry { get { return new Attribute("[Locations].[Location Country]"); } }
		[Tag("[Locations].[Location Email]")]
		public Attribute LocationEmail { get { return new Attribute("[Locations].[Location Email]"); } }
		[Tag("[Locations].[Location ID]")]
		public Attribute LocationID { get { return new Attribute("[Locations].[Location ID]"); } }
		[Tag("[Locations].[Location Name]")]
		public Attribute LocationName { get { return new Attribute("[Locations].[Location Name]"); } }
		[Tag("[Locations].[Location Phone]")]
		public Attribute LocationPhone { get { return new Attribute("[Locations].[Location Phone]"); } }
		[Tag("[Locations].[Location Postal]")]
		public Attribute LocationPostal { get { return new Attribute("[Locations].[Location Postal]"); } }
		[Tag("[Locations].[Location State]")]
		public Attribute LocationState { get { return new Attribute("[Locations].[Location State]"); } }
		[Tag("[Locations].[Location Status]")]
		public Attribute LocationStatus { get { return new Attribute("[Locations].[Location Status]"); } }
		[Tag("[Locations].[Location Street]")]
		public Attribute LocationStreet { get { return new Attribute("[Locations].[Location Street]"); } }
		[Tag("[Locations].[Location Type]")]
		public Attribute LocationType { get { return new Attribute("[Locations].[Location Type]"); } }
		[Tag("[Locations].[Location Website]")]
		public Attribute LocationWebsite { get { return new Attribute("[Locations].[Location Website]"); } }
		[Tag("[Locations].[Longitude]")]
		public Attribute Longitude { get { return new Attribute("[Locations].[Longitude]"); } }
		[Tag("[Locations].[Potential]")]
		public Attribute Potential { get { return new Attribute("[Locations].[Potential]"); } }
		[Tag("[Locations].[President]")]
		public Attribute President { get { return new Attribute("[Locations].[President]"); } }
		[Tag("[Locations].[President Name]")]
		public Attribute PresidentName { get { return new Attribute("[Locations].[President Name]"); } }
		[Tag("[Locations].[Size]")]
		public Attribute Size { get { return new Attribute("[Locations].[Size]"); } }
		[Tag("[Locations].[Store Locator ID]")]
		public Attribute StoreLocatorID { get { return new Attribute("[Locations].[Store Locator ID]"); } }
		[Tag("[Locations].[TM ID]")]
		public Attribute TMID { get { return new Attribute("[Locations].[TM ID]"); } }
		[Tag("[Locations].[TM Name]")]
		public Attribute TMName { get { return new Attribute("[Locations].[TM Name]"); } }
		[Tag("[Locations].[TM Territory]")]
		public Attribute TMTerritory { get { return new Attribute("[Locations].[TM Territory]"); } }
		[Tag("[Locations].[VP Sales]")]
		public Attribute VPSales { get { return new Attribute("[Locations].[VP Sales]"); } }
		[Tag("[Locations].[VP Sales Name]")]
		public Attribute VPSalesName { get { return new Attribute("[Locations].[VP Sales Name]"); } }
		
		[Hierarchy]
		public SalesHierarchy_Hierarchy SalesHierarchy { get { return new SalesHierarchy_Hierarchy(); } }
		
		public sealed class SalesHierarchy_Hierarchy : Hierarchy, ICubeObject
		{
			public SalesHierarchy_Hierarchy()
				: base("[Locations].[Sales Hierarchy]") { }

			[Tag("[Locations].[Sales Hierarchy].[President]", 1)]
			public Level President { get { return new Level("[Locations].[Sales Hierarchy].[President]", 1); } }
			[Tag("[Locations].[Sales Hierarchy].[VP Sales]", 2)]
			public Level VPSales { get { return new Level("[Locations].[Sales Hierarchy].[VP Sales]", 2); } }
			[Tag("[Locations].[Sales Hierarchy].[DSM Territory]", 3)]
			public Level DSMTerritory { get { return new Level("[Locations].[Sales Hierarchy].[DSM Territory]", 3); } }
			[Tag("[Locations].[Sales Hierarchy].[TM Territory]", 4)]
			public Level TMTerritory { get { return new Level("[Locations].[Sales Hierarchy].[TM Territory]", 4); } }
		}
	}

	[Dimension]
	public sealed class ReSight_Sales_Dimension : ICubeObject
	{
		[Tag("[Sales].[Item ID]")]
		public Attribute ItemID { get { return new Attribute("[Sales].[Item ID]"); } }
		[Tag("[Sales].[Rec ID]")]
		public Attribute RecID { get { return new Attribute("[Sales].[Rec ID]"); } }
	}

	[Dimension]
	public sealed class ReSight_TerritoryCluster_Dimension : ICubeObject
	{
		[Tag("[Territory Cluster].[Login]")]
		public Attribute Login { get { return new Attribute("[Territory Cluster].[Login]"); } }
		[Tag("[Territory Cluster].[Territory ID]")]
		public Attribute TerritoryID { get { return new Attribute("[Territory Cluster].[Territory ID]"); } }
		[Tag("[Territory Cluster].[TerritoryKey]")]
		public Attribute TerritoryKey { get { return new Attribute("[Territory Cluster].[TerritoryKey]"); } }
	}

	#endregion

	[Cube]
	public partial class ReSight : ICubeObject
	{
		public static ReSight Objects { get { return new ReSight(); } }
		public ReSight()
		{
			this.Calendar = new ReSight_Calendar_Dimension();
			this.ChainCluster = new ReSight_ChainCluster_Dimension();
			this.DistCluster = new ReSight_DistCluster_Dimension();
			this.DistributorDCs = new ReSight_DistributorDCs_Dimension();
			this.Distributors = new ReSight_Distributors_Dimension();
			this.DSMCluster = new ReSight_DSMCluster_Dimension();
			this.Items = new ReSight_Items_Dimension();
			this.Locations = new ReSight_Locations_Dimension();
			this.Sales = new ReSight_Sales_Dimension();
			this.TerritoryCluster = new ReSight_TerritoryCluster_Dimension();
		}

		public ReSight_Calendar_Dimension Calendar  { get; private set; }
		public ReSight_ChainCluster_Dimension ChainCluster  { get; private set; }
		public ReSight_DistCluster_Dimension DistCluster  { get; private set; }
		public ReSight_DistributorDCs_Dimension DistributorDCs  { get; private set; }
		public ReSight_Distributors_Dimension Distributors  { get; private set; }
		public ReSight_DSMCluster_Dimension DSMCluster  { get; private set; }
		public ReSight_Items_Dimension Items  { get; private set; }
		public ReSight_Locations_Dimension Locations  { get; private set; }
		public ReSight_Sales_Dimension Sales  { get; private set; }
		public ReSight_TerritoryCluster_Dimension TerritoryCluster  { get; private set; }
		[Measure("[Cust Units]")]
		public Measure CustUnits { get { return new Measure("Cust Units"); } }
		[Measure("[Cust Dollars]")]
		public Measure CustDollars { get { return new Measure("Cust Dollars"); } }
		[Measure("[Cust Weight]")]
		public Measure CustWeight { get { return new Measure("Cust Weight"); } }
		[Measure("[Unit Count]")]
		public Measure UnitCount { get { return new Measure("Unit Count"); } }
		[Measure("[SWP Dollars]")]
		public Measure SWPDollars { get { return new Measure("SWP Dollars"); } }
		[Measure("[SRP Dollars]")]
		public Measure SRPDollars { get { return new Measure("SRP Dollars"); } }
		[Measure("[Transaction Count]")]
		public Measure TransactionCount { get { return new Measure("Transaction Count"); } }
		[Measure("[Net Weight]")]
		public Measure NetWeight { get { return new Measure("Net Weight"); } }
	}
}
