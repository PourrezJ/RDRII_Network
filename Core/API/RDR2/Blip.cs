using Shared.Math;
using RDRN_Core.Native;
using System;

namespace RDRN_Core
{
	public sealed class Blip : PoolObject, IEquatable<Blip>
	{
		public Blip( int handle ) : base( handle ) {

		}

		public BlipSprite Sprite
		{
			set => Function.Call( Hash.SET_BLIP_SPRITE, Handle, (uint)value );
		}
		
		public void ModifierAdd(BlipModifier modifier)
		{
			 Function.Call(Hash._0x662D364ABF16DE2F, Handle, (uint)modifier);
		}
		
		public void ModifierRemove(BlipModifier modifier)
		{
			Function.Call(Hash._SET_BLIP_FLASH_STYLE, Handle, (uint)modifier);
		}
		
		public Vector2 Scale
		{
			set => Function.Call( Hash.SET_BLIP_SCALE, Handle, value.X, value.Y );
		}

		public string Label
		{
			set => Function.Call(Hash._SET_BLIP_NAME_FROM_PLAYER_STRING, Handle,
				Function.Call<string>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", value));
		}

		public bool IsFlashing
		{
			set => Function.Call( Hash.SET_BLIP_FLASHES, Handle, value, 2 );
		}

		public bool IsOnMinimap => Function.Call<bool>( Hash.IS_BLIP_ON_MINIMAP, Handle );

		public override bool Exists() {
			return Function.Call<bool>( Hash.DOES_BLIP_EXIST, Handle );
		}

		public override void Delete() {
			unsafe
			{
				int ptr = Handle;
				Function.Call(Hash.REMOVE_BLIP, &ptr);
			}
		}

		public bool Equals( Blip other ) {
			return !ReferenceEquals( null, other ) && other.Handle == Handle;
		}

		public override bool Equals( object obj ) {
			return ReferenceEquals( this, obj ) || obj is Blip other && Equals( other );
		}

		public override int GetHashCode() {
			return Handle.GetHashCode();
		}
	}

	/// <summary>
	/// The sprite icon to set the blip to. Here's a
	/// <see href="https://cdn.discordapp.com/attachments/450373719974477835/643937562091716638/unknown.png">reference image</see>
	/// in order from left-to-right by GlitchDetector, and each row indicates the different type.
	/// </summary>
	public enum BlipSprite : uint
	{
		Dominoes = 0x9D9FE583,
		DominoesAllFives = 0xFD189BDE,
		DominosAllThrees = 0xA1C2EBE4,
		FiveFingerFillet = 0x75B54B90,
		FiveFingerFilletGuts = 0x7869CF4,
		FiveFingerFilletBurnout = 0x3C88E424,
		Poker = 0x4A2357A9,
		SaddleBag = 0xB0E5E617,
		Doctor = 0x984E7CA9,
		EatingUtensils = 0x37BEBE4E,
		DonateToCamp = 0x8B7E38C4,
		ObjectiveChore = 0xDDFBA6AB,
		Ammunition = 0x5DF6DEBD,
		HealthSupplies = 0xD68D851B,
		Provisions = 0x919BC110,
		SmallBlackDot = 0x4ECB0761,
		Wheel = 0x3C5469D5,
		Stranger = 0x935EE440,
		Drinking = 0x4A0E7F51,
		HuntingGrounds = 0x1DCFAA8C,
		Fishing = 0xA216510E,
		MoneyWheel = 0xD4859AFE,
		Bank = 0x25249A47,
		Herd = 0x193BD50E,
		CaravanCamp = 0xA0417C98,
		HomeRobbery = 0x1A7A040D,
		
		BLIP_AMBIENT_BOUNTY_HUNTER = 0xCCAAD634,
		BLIP_AMBIENT_BOUNTY_TARGET = 0x5846C31D,
		BLIP_AMBIENT_CHORE = 0x4ECB0761,
		BLIP_AMBIENT_COACH = 0x3C5469D5,
		BLIP_AMBIENT_COACH_TAXI = 0x1237009A,
		BLIP_AMBIENT_COMPANION = 0xF4F30880,
		BLIP_AMBIENT_CORPSE = 0xBD7800C3,
		BLIP_AMBIENT_DEATH = 0x14E5460D,
		BLIP_AMBIENT_EYEWITNESS = 0x87B23EE0,
		BLIP_AMBIENT_GANG_LEADER = 0xA73D2720,
		BLIP_AMBIENT_HERD = 0x193BD50E,
		BLIP_AMBIENT_HERD_STRAGGLER = 0x8A089DA6,
		BLIP_AMBIENT_HITCHING_POST = 0x48C3FC57,
		BLIP_AMBIENT_HORSE = 0xD99F0C2B,
		BLIP_AMBIENT_LAW = 0xA0D367A5,
		BLIP_AMBIENT_LOAN_SHARK = 0x6D930ED3,
		BLIP_AMBIENT_NEW = 0x18FD604D,
		BLIP_AMBIENT_NEWSPAPER = 0x23098844,
		BLIP_AMBIENT_NPC = 0x1232388E,
		BLIP_AMBIENT_PED_DOWNED = 0x56A6BAD2,
		BLIP_AMBIENT_PED_MEDIUM = 0xAF7CFC61,
		BLIP_AMBIENT_PED_MEDIUM_HIGHER = 0x529D2978,
		BLIP_AMBIENT_PED_MEDIUM_LOWER = 0x76F6E1CA,
		BLIP_AMBIENT_PED_SMALL = 0xA9056,
		BLIP_AMBIENT_PLANT = 0x34E12F3F,
		BLIP_AMBIENT_PLANT_NEW = 0xC6648164,
		BLIP_AMBIENT_QUARTERMASTER = 0xEE27357,
		BLIP_AMBIENT_RIVERBOAT = 0x79332DAE,
		BLIP_AMBIENT_SECRET = 0x28437426,
		BLIP_AMBIENT_SHERIFF = 0xD6A7D13B,
		BLIP_AMBIENT_TELEGRAPH = 0x1DFBEC1C,
		BLIP_AMBIENT_THEATRE = 0xE716BC25,
		BLIP_AMBIENT_TITHING = 0x8B7E38C4,
		BLIP_AMBIENT_TRACKING = 0xA1CB4518,
		BLIP_AMBIENT_TRAIN = 0xF1119380,
		BLIP_AMBIENT_WAGON = 0x341C1421,
		BLIP_AMBIENT_WARP = 0x2EBE3826,
		BLIP_AMMO_ARROW = 0x2232765B,
		BLIP_ANIMAL = 0x9DE00913,
		BLIP_ANIMAL_DEAD = 0x4FE13DF7,
		BLIP_ANIMAL_QUALITY_01 = 0x7702FDE0,
		BLIP_ANIMAL_QUALITY_02 = 0xF5CD7B77,
		BLIP_ANIMAL_QUALITY_03 = 0xE35F569B,
		BLIP_ANIMAL_SKIN = 0xD047184,
		BLIP_APP_CONNECTED = 0x45F625B9,
		BLIP_AREA_EDGE_DIRECTIONAL = 0xF193103E,
		BLIP_ATTENTION = 0xD1D3320F,
		BLIP_BANK_DEBT = 0x6F6A7070,
		BLIP_BATH_HOUSE = 0xEDD78E2F,
		BLIP_CAMP = 0x138060D3,
		BLIP_CAMP_REQUEST = 0xC1C80785,
		BLIP_CAMP_TENT = 0xC9C26F22,
		BLIP_CAMPFIRE = 0x68917D2D,
		BLIP_CAMPFIRE_FULL = 0x2E1C03FA,
		BLIP_CANOE = 0x3B87AAB,
		BLIP_CASH_ARTHUR = 0x54A5D841,
		BLIP_CASH_BAG = 0x290B09DE,
		BLIP_CHEST = 0xBC1E4FC8,
		BLIP_CODE_AREA = 0x3A234006,
		BLIP_CODE_AREA_SOLID = 0x7AC64DF9,
		BLIP_CODE_CENTER = 0xD2CB22A7,
		BLIP_CODE_CENTER_ON_HORSE = 0x26A0B9BB,
		BLIP_CODE_LEVEL = 0x448CEB1C,
		BLIP_CODE_LEVEL_AREA = 0x6E5178D7,
		BLIP_CODE_RADIUS = 0xDCECC828,
		BLIP_CODE_RADIUS_OUTLINE = 0xDE204641,
		BLIP_CODE_WANTED_RADIUS = 0xBB601A2E,
		BLIP_CODE_WAYPOINT = 0x393F91E2,
		BLIP_DEADEYE_CROSS = 0x6893A647,
		BLIP_DIRECTION_POINTER = 0x31946E8,
		BLIP_DONATE_FOOD = 0xB653DC5B,
		BLIP_EVENT_APPLESEED = 0x7183BF3C,
		BLIP_EVENT_CASTOR = 0x896733B6,
		BLIP_EVENT_RAILROAD_CAMP = 0xE2EF5384,
		BLIP_EVENT_RIGGS_CAMP = 0x8C1AE2A6,
		BLIP_FENCE_BUILDING = 0xB9B66375,
		BLIP_FOR_SALE = 0xAD9089F6,
		BLIP_GANG_SAVINGS = 0x2209BCE9,
		BLIP_GANG_SAVINGS_SPECIAL = 0x507D36D9,
		BLIP_GRUB = 0x37BEBE4E,
		BLIP_HAT = 0x3B0C645A,
		BLIP_HORSE_OWNED = 0x99C448B5,
		BLIP_HORSE_OWNED_ACTIVE = 0x4821A7BB,
		BLIP_HORSE_OWNED_BONDING_0 = 0xF30AE681,
		BLIP_HORSE_OWNED_BONDING_1 = 0xD58216,
		BLIP_HORSE_OWNED_BONDING_2 = 0x179FAFAA,
		BLIP_HORSE_OWNED_BONDING_3 = 0x25234AB1,
		BLIP_HORSE_OWNED_BONDING_4 = 0xDA01B467,
		BLIP_HORSE_OWNED_HITCHED = 0xFD52BABC,
		BLIP_HORSE_TEMP = 0xD9C50D7B,
		BLIP_HORSE_TEMP_BONDING_0 = 0x37E1EFF6,
		BLIP_HORSE_TEMP_BONDING_1 = 0x1D30BA94,
		BLIP_HORSE_TEMP_BONDING_2 = 0xBA29778,
		BLIP_HORSE_TEMP_BONDING_3 = 0xF9D5F3DF,
		BLIP_HORSE_TEMP_BONDING_4 = 0xCF619EFB,
		BLIP_HORSE_TEMP_HITCHED = 0x1A82264C,
		BLIP_HORSESHOE_0 = 0x707A997A,
		BLIP_HORSESHOE_1 = 0x7D39B2F8,
		BLIP_HORSESHOE_2 = 0x4584C39F,
		BLIP_HORSESHOE_3 = 0x573D6710,
		BLIP_HORSESHOE_4 = 0x28F70A84,
		BLIP_HOTEL_BED = 0xF363E60C,
		BLIP_JOB = 0xC52EB282,
		BLIP_LOCKED = 0x4AD28B8C,
		BLIP_MG_BLACKJACK = 0x23837E0A,
		BLIP_MG_DOMINOES = 0x9D9FE583,
		BLIP_MG_DOMINOES_ALL3S = 0xA1C2EBE4,
		BLIP_MG_DOMINOES_ALL5S = 0xFD189BDE,
		BLIP_MG_DOMINOES_DRAW = 0xE96742F2,
		BLIP_MG_DRINKING = 0x4A0E7F51,
		BLIP_MG_FISHING = 0xA216510E,
		BLIP_MG_FIVE_FINGER_FILLET = 0x75B54B90,
		BLIP_MG_FIVE_FINGER_FILLET_BURNOUT = 0x3C88E424,
		BLIP_MG_FIVE_FINGER_FILLET_GUTS = 0x7869CF4,
		BLIP_MG_POKER = 0x4A2357A9,
		BLIP_MISSION_AREA_BEAU = 0xA7F0BFDC,
		BLIP_MISSION_AREA_BILL = 0x1B2119E8,
		BLIP_MISSION_AREA_BOUNTY = 0x7EAB2A55,
		BLIP_MISSION_AREA_BRONTE = 0xD6201C07,
		BLIP_MISSION_AREA_DAVID_GEDDES = 0x53AD4861,
		BLIP_MISSION_AREA_DUTCH = 0x6717F6BA,
		BLIP_MISSION_AREA_EAGLE_FLIES = 0x1E52B336,
		BLIP_MISSION_AREA_EDITH = 0xE301FA16,
		BLIP_MISSION_AREA_GRAYS = 0x6FDF1545,
		BLIP_MISSION_AREA_GUNSLINGER_1 = 0x91C4791A,
		BLIP_MISSION_AREA_GUNSLINGER_2 = 0x633F9C11,
		BLIP_MISSION_AREA_HENRI = 0x3E7A8FC8,
		BLIP_MISSION_AREA_HOSEA = 0x2EB9EE49,
		BLIP_MISSION_AREA_JAVIER = 0x1D8C0C12,
		BLIP_MISSION_AREA_JOHN = 0xDD1168D,
		BLIP_MISSION_AREA_KITTY = 0x3BC91DD2,
		BLIP_MISSION_AREA_LEON = 0xD712FE29,
		BLIP_MISSION_AREA_LIGHTNING = 0x40A77909,
		BLIP_MISSION_AREA_LOANSHARK = 0xC259BAAF,
		BLIP_MISSION_AREA_MARY = 0xC8D9E017,
		BLIP_MISSION_AREA_MICAH = 0xC5B8E7A1,
		BLIP_MISSION_AREA_RAINS = 0x4ACE007B,
		BLIP_MISSION_AREA_RC = 0x88DB3581,
		BLIP_MISSION_AREA_REVEREND = 0xF637407D,
		BLIP_MISSION_AREA_SADIE = 0x61402C2B,
		BLIP_MISSION_AREA_STRAUSS = 0xD502C76F,
		BLIP_MISSION_AREA_TRELAWNEY = 0xB5BE2243,
		BLIP_MISSION_ARTHUR = 0x7542B36A,
		BLIP_MISSION_BG = 0xF888671C,
		BLIP_MISSION_BILL = 0x3850B0AA,
		BLIP_MISSION_BOUNTY = 0xC9ED294C,
		BLIP_MISSION_CAMP = 0xBCF02D27,
		BLIP_MISSION_DUTCH = 0xF9A61C9E,
		BLIP_MISSION_HOSEA = 0x99393F16,
		BLIP_MISSION_JOHN = 0xCB14042D,
		BLIP_MISSION_MICAH = 0x4B8AB55B,
		BLIP_MP_PICKUP = 0x421F5035,
		BLIP_MP_WEAPON = 0x4C92880C,
		BLIP_NPC_SEARCH = 0x7915E848,
		BLIP_NULL = 0xCB96F80D,
		BLIP_OBJECTIVE = 0xDDFBA6AB,
		BLIP_OBJECTIVE_MINOR = 0x470E95D9,
		BLIP_OVERLAY_1 = 0x1CA74792,
		BLIP_OVERLAY_2 = 0xEE0FEA60,
		BLIP_OVERLAY_3 = 0x788F35,
		BLIP_OVERLAY_4 = 0x63D955F5,
		BLIP_OVERLAY_5 = 0x6E0C6A5B,
		BLIP_OVERLAY_BILL = 0x40F2263,
		BLIP_OVERLAY_CHARLES = 0xD7CF1CF8,
		BLIP_OVERLAY_HORSE_REVIVE = 0x40561B75,
		BLIP_OVERLAY_HOSEA = 0xC53E4D72,
		BLIP_OVERLAY_JAVIER = 0x1D8800FD,
		BLIP_OVERLAY_JOHN = 0x96D989FF,
		BLIP_OVERLAY_KAREN = 0xB378549F,
		BLIP_OVERLAY_KIERAN = 0x226F32AA,
		BLIP_OVERLAY_LENNY = 0xA71C76DA,
		BLIP_OVERLAY_LOANSHARK = 0x99DFD7AB,
		BLIP_OVERLAY_MICAH = 0x303DD65B,
		BLIP_OVERLAY_PARTY = 0x4166EF86,
		BLIP_OVERLAY_PEARSON = 0x40932364,
		BLIP_OVERLAY_REVIVE = 0x8D2C8249,
		BLIP_OVERLAY_RING = 0xF4FDCFA6,
		BLIP_OVERLAY_SEAN = 0xD8E1A3E1,
		BLIP_OVERLAY_STRAUSS = 0x67969C68,
		BLIP_OVERLAY_TILLY = 0x77C1D895,
		BLIP_OVERLAY_UNCLE = 0x9A41F739,
		BLIP_OVERLAY_WHITE_1 = 0xCFB84B8F,
		BLIP_OVERLAY_WHITE_2 = 0xE171EF02,
		BLIP_OVERLAY_WHITE_3 = 0xB46994F2,
		BLIP_OVERLAY_WHITE_4 = 0xC62FB87E,
		BLIP_OVERLAY_WHITE_5 = 0x48D63DC5,
		BLIP_PHOTO_STUDIO = 0x514D700D,
		BLIP_PLANT = 0xD7BA5EA3,
		BLIP_PLAYER = 0xE0C59962,
		BLIP_PLAYER_COACH = 0xEA75A451,
		BLIP_POI = 0x866B73BE,
		BLIP_POST_OFFICE = 0x6EECC2CD,
		BLIP_POST_OFFICE_REC = 0x57F08E7F,
		BLIP_PROC_BANK = 0x8128776F,
		BLIP_PROC_BOUNTY_POSTER = 0x9E6FEC8F,
		BLIP_PROC_HOME = 0x5E8C9DD0,
		BLIP_PROC_HOME_LOCKED = 0xA6ABB3F7,
		BLIP_PROC_LOANSHARK = 0xC49121DE,
		BLIP_PROC_TRACK = 0x1918D829,
		BLIP_RADAR_EDGE_POINTER = 0xB8E49AC7,
		BLIP_RADIUS_SEARCH = 0x8F78F91,
		BLIP_RC = 0x935EE440,
		BLIP_RC_ALBERT = 0xB4EAACC6,
		BLIP_RC_ALGERNON_WASP = 0x7DA4AB60,
		BLIP_RC_ART = 0xE61B649E,
		BLIP_RC_CALLOWAY = 0x980696BF,
		BLIP_RC_CHAIN_GANG = 0xDADE83D7,
		BLIP_RC_CHARLOTTE_BALFOUR = 0x9C0D8E6E,
		BLIP_RC_CRACKPOT = 0x345284D0,
		BLIP_RC_DEBORAH = 0x4547591A,
		BLIP_RC_DOCTOR = 0x294D7F73,
		BLIP_RC_GUNSLINGER_1 = 0x33295DF0,
		BLIP_RC_GUNSLINGER_2 = 0x1C9630CA,
		BLIP_RC_GUNSLINGER_3 = 0xE549447,
		BLIP_RC_GUNSLINGER_5 = 0x6C18CFCE,
		BLIP_RC_HENRI = 0xEBB45BB5,
		BLIP_RC_HOBBS = 0x76679173,
		BLIP_RC_JEREMY_GILL = 0xEFB759BE,
		BLIP_RC_KITTY = 0x756CBF95,
		BLIP_RC_LIGHTNING = 0x8B06EC18,
		BLIP_RC_OBEDIAH_HINTON = 0x18C1FFE8,
		BLIP_RC_ODD_FELLOWS = 0x313B2909,
		BLIP_RC_OH_BROTHER = 0xB3B0A1C3,
		BLIP_RC_OLD_FLAME = 0x5535FA8F,
		BLIP_RC_SLAVE_CATCHER = 0xB9EBFE5,
		BLIP_RC_WAR_VETERAN = 0x69853262,
		BLIP_REGION_CARAVAN = 0xA0417C98,
		BLIP_REGION_HIDEOUT = 0xE66E67CE,
		BLIP_REGION_HUNTING = 0x1DCFAA8C,
		BLIP_ROBBERY_BANK = 0x25249A47,
		BLIP_ROBBERY_COACH = 0xD4859AFE,
		BLIP_ROBBERY_HOME = 0x1A7A040D,
		BLIP_RPG_OVERWEIGHT = 0xBDF6233A,
		BLIP_RPG_UNDERWEIGHT = 0x424276A8,
		BLIP_SADDLE = 0xB0E5E617,
		BLIP_SALOON = 0x70033BCC,
		BLIP_SCM_ABE_STABLEHAND = 0x1BFF0ECA,
		BLIP_SCM_ABIGAIL = 0x7F3C1B85,
		BLIP_SCM_ALBERT_CAKES = 0x7EB381AF,
		BLIP_SCM_ANDREAS = 0xFBE35E35,
		BLIP_SCM_ANSEL_ATHERTON = 0xF46C6D41,
		BLIP_SCM_BEAU = 0x4D8DF269,
		BLIP_SCM_BRONTE = 0xBC311FE1,
		BLIP_SCM_CALDERON = 0xCFC5995C,
		BLIP_SCM_CHARLES = 0xAB5E836C,
		BLIP_SCM_DAVID_GEDDES = 0x99C2B1DA,
		BLIP_SCM_DORKINS = 0x13CFB2E9,
		BLIP_SCM_EAGLE_FLIES = 0x405ED49A,
		BLIP_SCM_EDITH = 0xD897A212,
		BLIP_SCM_EVELYN = 0x9FFC54F5,
		BLIP_SCM_FRANCES = 0xD16A7954,
		BLIP_SCM_GRAYS = 0xC56EA4F6,
		BLIP_SCM_JACK = 0x172A9DB7,
		BLIP_SCM_JAVIER = 0x848681B3,
		BLIP_SCM_KIERAN = 0xE6EE6293,
		BLIP_SCM_LENNY = 0x30CCC863,
		BLIP_SCM_LEON = 0xE2B6FC96,
		BLIP_SCM_LETTER = 0x82CB9F86,
		BLIP_SCM_MARY = 0x5711F8D5,
		BLIP_SCM_MARYBETH = 0x247737D9,
		BLIP_SCM_MOLLY_OSHEA = 0x4DD8BB86,
		BLIP_SCM_MONROE = 0xEE0026A1,
		BLIP_SCM_PEARSON = 0xD64E7A5E,
		BLIP_SCM_PENELOPE = 0xE19907E5,
		BLIP_SCM_RAINS = 0xF0402309,
		BLIP_SCM_REVEREND = 0xD7BB296A,
		BLIP_SCM_SADIE = 0x497B7ADA,
		BLIP_SCM_SEAN = 0x5F49DA0B,
		BLIP_SCM_STRAUSS = 0x7FE16A19,
		BLIP_SCM_SUSAN = 0x3C384A57,
		BLIP_SCM_TOM_DICKENS = 0xB2C0DCCA,
		BLIP_SCM_TRELAWNEY = 0xBB5DD71B,
		BLIP_SHOP_BARBER = 0x8365EAEC,
		BLIP_SHOP_BLACKSMITH = 0xD2C3066D,
		BLIP_SHOP_BUTCHER = 0x9CBBB93B,
		BLIP_SHOP_COACH_FENCING = 0x896D974C,
		BLIP_SHOP_DOCTOR = 0x984E7CA9,
		BLIP_SHOP_GUNSMITH = 0xF74E39B1,
		BLIP_SHOP_HORSE = 0x738F7AAF,
		BLIP_SHOP_HORSE_FENCING = 0xA9340072,
		BLIP_SHOP_HORSE_SADDLE = 0x1C00FEF5,
		BLIP_SHOP_MARKET_STALL = 0x30DB3AC6,
		BLIP_SHOP_SHADY_STORE = 0x1FAA7FEA,
		BLIP_SHOP_STORE = 0x57F823F2,
		BLIP_SHOP_TACKLE = 0xCD33D526,
		BLIP_SHOP_TAILOR = 0x474561EC,
		BLIP_SHOP_TRAIN = 0x62B22FA,
		BLIP_SHOP_TRAINER = 0x5BED407C,
		BLIP_STYLE_PING_MEDIUM_SMALL = 0x4C3C857E,
		BLIP_STYLE_TURRET_WEAPON = 0x4C0EF147,
		BLIP_STYLE_UNDISCOVERED = 0xD2D71097,
		BLIP_STYLE_UPCOMING_OBJECTIVE = 0x289A47CD,
		BLIP_SUMMER_COW = 0x404B2E7B,
		BLIP_SUMMER_FEED = 0x27E4D337,
		BLIP_SUMMER_HORSE = 0x20F0E989,
		BLIP_SUPPLIES_AMMO = 0x5DF6DEBD,
		BLIP_SUPPLIES_FOOD = 0x919BC110,
		BLIP_SUPPLIES_HEALTH = 0xD68D851B,
		BLIP_SUPPLY_ICON_AMMO = 0x5231B9FE,
		BLIP_SUPPLY_ICON_FOOD = 0x189CC849,
		BLIP_SUPPLY_ICON_HEALTH = 0xC8EC8DC9,
		BLIP_SWAP = 0xA644C4BF,
		BLIP_TAXIDERMIST = 0x98AC580D,
		BLIP_TIME_OF_DAY = 0xE9F6A610,
		BLIP_TOWN = 0xB4FBA463,
		BLIP_WEAPON = 0x20C38D85,
		BLIP_WEAPON_BOW = 0xF81C3313,
		BLIP_WEAPON_CANNON = 0xEA552CD8,
		BLIP_WEAPON_DYNAMITE = 0x4313C563,
		BLIP_WEAPON_GATLING = 0x7E6B3246,
		BLIP_WEAPON_HANDGUN = 0xC1462614,
		BLIP_WEAPON_LONGARM = 0xC4A70894,
		BLIP_WEAPON_MELEE = 0x1DE1954C,
		BLIP_WEAPON_MOLOTOV = 0x1F5D9079,
		BLIP_WEAPON_SHOTGUN = 0x94E8CD14,
		BLIP_WEAPON_SNIPER = 0x9E9254C5,
		BLIP_WEAPON_THROWABLE = 0x585E4402,
		BLIP_WEAPON_THROWING_KNIFE = 0x36248ED6,
		BLIP_WEAPON_TOMAHAWK = 0x58460877,
		BLIP_WEAPON_TORCH = 0x7E5BCB24,
	}
	public enum BlipModifier : uint
	{
		BLIP_MODIFIER_AREA = 0xA2814CC7,
		BLIP_MODIFIER_AREA_CLAMPED_PULSE = 0xA0A765A1,
		BLIP_MODIFIER_AREA_CONTESTED = 0x6E1AE519,
		BLIP_MODIFIER_AREA_DIRECTIONAL = 0x38E24039,
		BLIP_MODIFIER_AREA_HIDE_ON_INSIDE = 0xAD9CBA59,
		BLIP_MODIFIER_AREA_HIDE_ON_OUTSIDE = 0xD4BC0DD7,
		BLIP_MODIFIER_AREA_OUT_OF_BOUNDS = 0xAB81D4D6,
		BLIP_MODIFIER_AREA_PULSE = 0x1BE311B3,
		BLIP_MODIFIER_AREA_TAKEOVER = 0x3494D1C8,
		BLIP_MODIFIER_ATTENTION = 0x240DE18,
		BLIP_MODIFIER_BOUNTY_TARGET_INCAPACITATED = 0xF51B7DD8,
		BLIP_MODIFIER_COMPANION_ACTIVITY = 0x56B8B889,
		BLIP_MODIFIER_COMPANION_CONVERSATION = 0xB4CA5F4C,
		BLIP_MODIFIER_COMPANION_DOG = 0xB93E613,
		BLIP_MODIFIER_COMPANION_OBJECTIVE = 0xDF69E371,
		BLIP_MODIFIER_COMPANION_WOUNDED = 0x18006B2F,
		BLIP_MODIFIER_COMPASS_OBJECTIVE = 0xDC7BE1A,
		BLIP_MODIFIER_CREATOR_FOCUS = 0x557F109C,
		BLIP_MODIFIER_CULL_ON_DEATH = 0xAA235D8B,
		BLIP_MODIFIER_DIRECTION_ONLY = 0x5824DF7B,
		BLIP_MODIFIER_DISTANCE_FADE_LONG = 0x8F934A09,
		BLIP_MODIFIER_DISTANCE_FADE_MEDIUM = 0x23BCF4A2,
		BLIP_MODIFIER_DISTANCE_FADE_SHORT = 0x111DDB96,
		BLIP_MODIFIER_ENEMY = 0x382616F3,
		BLIP_MODIFIER_ENEMY_AQUATIC = 0x4BCEFC2C,
		BLIP_MODIFIER_ENEMY_CONFRONTING = 0x2CB39D87,
		BLIP_MODIFIER_ENEMY_DISAPPEARING = 0x7CFAB4C0,
		BLIP_MODIFIER_ENEMY_DISAPPEARING_NO_FLEE_CHECKS = 0xFCDCDBA,
		BLIP_MODIFIER_ENEMY_DISAPPEARING_NO_SCREEN_CHECKS = 0x1852773,
		BLIP_MODIFIER_ENEMY_FLEEING = 0x1FDAEC84,
		BLIP_MODIFIER_ENEMY_GUNSHOTS_FADE_ON_START = 0x57F061DF,
		BLIP_MODIFIER_ENEMY_GUNSHOTS_ONLY = 0xC256FEAF,
		BLIP_MODIFIER_ENEMY_GUNSHOTS_ONLY_DONT_SHOW_LAST = 0x27CCDA52,
		BLIP_MODIFIER_ENEMY_IS_ALERTED = 0xF85DCD5A,
		BLIP_MODIFIER_ENEMY_LOWER_AWARENESS = 0x50235612,
		BLIP_MODIFIER_ENEMY_MUST_AGGRO = 0x69392A93,
		BLIP_MODIFIER_ENEMY_ON_GUARD = 0xD886D9BD,
		BLIP_MODIFIER_ENEMY_ON_GUARD_DISAPPEARING = 0x5C6421C6,
		BLIP_MODIFIER_ENEMY_STEALTH_KILL = 0xBD592C8A,
		BLIP_MODIFIER_ENEMY_TARGETED_ONLY = 0xE9CA636F,
		BLIP_MODIFIER_FADE = 0xBC9C3EA,
		BLIP_MODIFIER_FADE_OUT_AND_DIE = 0xB452F7BC,
		BLIP_MODIFIER_FADE_OUT_SLOW = 0x5426DAB6,
		BLIP_MODIFIER_FETCH_ESCAPING = 0xE6239722,
		BLIP_MODIFIER_FLASH_FOREVER = 0xC1DBF36F,
		BLIP_MODIFIER_FOR_SALE = 0xDAA61911,
		BLIP_MODIFIER_FORCE_GPS = 0x900A4D0A,
		BLIP_MODIFIER_FRIENDLY = 0x6414AA9A,
		BLIP_MODIFIER_FRIENDLY_OBJECTIVE = 0xC586CF7A,
		BLIP_MODIFIER_HIDDEN = 0xB946AEF0,
		BLIP_MODIFIER_HIDEOUT_ABANDONED = 0x9916A554,
		BLIP_MODIFIER_HIGH_CATEGORY = 0xCCC27837,
		BLIP_MODIFIER_JOB = 0x845A1E41,
		BLIP_MODIFIER_JOB_BILL = 0x5A9CF68D,
		BLIP_MODIFIER_JOB_CHARLES = 0x382968CB,
		BLIP_MODIFIER_JOB_HOSEA = 0x24D547E,
		BLIP_MODIFIER_JOB_JAVIER = 0xA66B982F,
		BLIP_MODIFIER_JOB_JOHN = 0x7EA2F90A,
		BLIP_MODIFIER_JOB_KAREN = 0x955011CB,
		BLIP_MODIFIER_JOB_KIERAN = 0x78BE3FBC,
		BLIP_MODIFIER_JOB_LENNY = 0x81F28880,
		BLIP_MODIFIER_JOB_LOANSHARK = 0x1D8CAE74,
		BLIP_MODIFIER_JOB_MICAH = 0xA26E278F,
		BLIP_MODIFIER_JOB_PEARSON = 0xC5B01DDB,
		BLIP_MODIFIER_JOB_SEAN = 0x31FDE57D,
		BLIP_MODIFIER_JOB_STRAUSS = 0x4FEF10D0,
		BLIP_MODIFIER_JOB_TILLY = 0xCAA7A516,
		BLIP_MODIFIER_JOB_UNCLE = 0xAAD9F18,
		BLIP_MODIFIER_KEY_JOB = 0x27045619,
		BLIP_MODIFIER_LAW_ORDER = 0x96AEC03E,
		BLIP_MODIFIER_LOCAL_PLAYER_OWNED = 0xB3892473,
		BLIP_MODIFIER_LOCKED = 0x2B30E11F,
		BLIP_MODIFIER_LOS_DISAPPEARING = 0x1C65CE16,
		BLIP_MODIFIER_MISSION_UNAVAILABLE = 0x821511C0,
		BLIP_MODIFIER_MP_COLOR_1 = 0x1DD3A06B,
		BLIP_MODIFIER_MP_COLOR_10 = 0x6A44C8F2,
		BLIP_MODIFIER_MP_COLOR_11 = 0xA6964198,
		BLIP_MODIFIER_MP_COLOR_12 = 0x57E2242D,
		BLIP_MODIFIER_MP_COLOR_13 = 0x8A3D88E7,
		BLIP_MODIFIER_MP_COLOR_14 = 0xB77BE363,
		BLIP_MODIFIER_MP_COLOR_15 = 0xEF61D32E,
		BLIP_MODIFIER_MP_COLOR_16 = 0xA1A7B7BB,
		BLIP_MODIFIER_MP_COLOR_17 = 0xD2CF1A09,
		BLIP_MODIFIER_MP_COLOR_18 = 0x518FE9C,
		BLIP_MODIFIER_MP_COLOR_19 = 0x38AB65C4,
		BLIP_MODIFIER_MP_COLOR_2 = 0x6F85C3CE,
		BLIP_MODIFIER_MP_COLOR_20 = 0x121C9983,
		BLIP_MODIFIER_MP_COLOR_21 = 0x7D55EFF4,
		BLIP_MODIFIER_MP_COLOR_22 = 0x6700C34A,
		BLIP_MODIFIER_MP_COLOR_23 = 0x8EA29299,
		BLIP_MODIFIER_MP_COLOR_24 = 0xA3F7BD43,
		BLIP_MODIFIER_MP_COLOR_25 = 0xF351DBF6,
		BLIP_MODIFIER_MP_COLOR_26 = 0xF85BE60A,
		BLIP_MODIFIER_MP_COLOR_27 = 0xCCED8F2E,
		BLIP_MODIFIER_MP_COLOR_28 = 0xDB4D2BED,
		BLIP_MODIFIER_MP_COLOR_29 = 0x29894868,
		BLIP_MODIFIER_MP_COLOR_3 = 0x8D2FFF22,
		BLIP_MODIFIER_MP_COLOR_30 = 0xE643BEC2,
		BLIP_MODIFIER_MP_COLOR_31 = 0xFEDE6FF7,
		BLIP_MODIFIER_MP_COLOR_32 = 0xEF1F5079,
		BLIP_MODIFIER_MP_COLOR_4 = 0xECA93E13,
		BLIP_MODIFIER_MP_COLOR_5 = 0x3A585978,
		BLIP_MODIFIER_MP_COLOR_6 = 0x481674F4,
		BLIP_MODIFIER_MP_COLOR_7 = 0x15BD9043,
		BLIP_MODIFIER_MP_COLOR_8 = 0xA1B5A82D,
		BLIP_MODIFIER_MP_COLOR_9 = 0xF376CBAE,
		BLIP_MODIFIER_MP_DOWNED = 0x24CB3FB5,
		BLIP_MODIFIER_MP_ENEMY_HOLDING = 0x738B1D05,
		BLIP_MODIFIER_MP_FRIENDLY_HOLDING = 0x22E21DB2,
		BLIP_MODIFIER_MP_HOT_BLIP = 0x2B389337,
		BLIP_MODIFIER_MP_HUNTER = 0x56AA576C,
		BLIP_MODIFIER_MP_JOB_PLAYER_FADE = 0xB87AC128,
		BLIP_MODIFIER_MP_NEUTRAL = 0x7C687658,
		BLIP_MODIFIER_MP_OBJECTIVE = 0xE0E7C82B,
		BLIP_MODIFIER_MP_OBJECTIVE_ENEMY = 0x801DD820,
		BLIP_MODIFIER_MP_OBJECTIVE_FRIENDLY = 0xA9DBBFDC,
		BLIP_MODIFIER_MP_OBJECTIVE_NEUTRAL = 0x9E703B63,
		BLIP_MODIFIER_MP_PLAYER_ALLY = 0xB1AE1182,
		BLIP_MODIFIER_MP_PLAYER_ALLY_WOUNDED = 0xDAC99B52,
		BLIP_MODIFIER_MP_PLAYER_CONTROL = 0x3A6AF541,
		BLIP_MODIFIER_MP_PLAYER_COOP = 0xA4F9F040,
		BLIP_MODIFIER_MP_PLAYER_DISAPPEARING = 0x40FB8A3E,
		BLIP_MODIFIER_MP_PLAYER_ENEMY = 0xDB76C121,
		BLIP_MODIFIER_MP_PLAYER_LOS_ONLY = 0xFDB13448,
		BLIP_MODIFIER_MP_PLAYER_NEUTRAL = 0x5F947624,
		BLIP_MODIFIER_MP_PLAYER_WINNING = 0xE0F654CC,
		BLIP_MODIFIER_MP_PLAYER_WITH_BOUNTY = 0x2B28A10C,
		BLIP_MODIFIER_MP_REVIVE = 0x9125D4D4,
		BLIP_MODIFIER_MP_RIVAL_RACER = 0x3599864F,
		BLIP_MODIFIER_MP_TEAM_COLOR_1 = 0x9B795DF5,
		BLIP_MODIFIER_MP_TEAM_COLOR_2 = 0x4F530F3,
		BLIP_MODIFIER_MP_TEAM_COLOR_3 = 0x232C6D61,
		BLIP_MODIFIER_MP_TEAM_COLOR_4 = 0x6876F7FD,
		BLIP_MODIFIER_MP_TEAM_COLOR_5 = 0xF6B79478,
		BLIP_MODIFIER_MP_TEAM_COLOR_6 = 0xDBC05E8A,
		BLIP_MODIFIER_MP_TEAM_COLOR_7 = 0xEA06FB17,
		BLIP_MODIFIER_MP_TEAM_COLOR_8 = 0xAFF286EF,
		BLIP_MODIFIER_MP_WHITE_FLAG = 0x35333C20,
		BLIP_MODIFIER_NEUTRAL_ON_GUARD = 0x4FCB6ECC,
		BLIP_MODIFIER_OBJECTIVE = 0xE80A86F4,
		BLIP_MODIFIER_OUTSIDE_TOD = 0xA9C5EBA4,
		BLIP_MODIFIER_OVERLAY_1 = 0xE7DF610D,
		BLIP_MODIFIER_OVERLAY_2 = 0xFD868C57,
		BLIP_MODIFIER_OVERLAY_3 = 0xBD028EA,
		BLIP_MODIFIER_OVERLAY_4 = 0xA30F576E,
		BLIP_MODIFIER_OVERLAY_5 = 0xB151F3F3,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_1 = 0xD9349943,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_10 = 0x9EB41558,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_2 = 0x71284930,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_3 = 0x5FBB2656,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_4 = 0x2662339D,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_5 = 0x8434EF49,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_6 = 0x4FA0861D,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_7 = 0xBE67E3AA,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_8 = 0x74354F46,
		BLIP_MODIFIER_OVERLAY_RING_MP_COLOR_9 = 0x60E428A4,
		BLIP_MODIFIER_OVERLAY_WHITE_1 = 0x508D5857,
		BLIP_MODIFIER_OVERLAY_WHITE_2 = 0x7B482DD0,
		BLIP_MODIFIER_OVERLAY_WHITE_3 = 0xDD10F160,
		BLIP_MODIFIER_OVERLAY_WHITE_4 = 0x99B56AAA,
		BLIP_MODIFIER_OVERLAY_WHITE_5 = 0x8BC4CEC9,
		BLIP_MODIFIER_PARTY = 0xD80C073B,
		BLIP_MODIFIER_PICKUP_NEW = 0x2818329D,
		BLIP_MODIFIER_POSSE_ALLY = 0xAD76DF0A,
		BLIP_MODIFIER_POSSE_ALLY_OWNED = 0x1B05ACFB,
		BLIP_MODIFIER_POSSE_ENEMY = 0xD2FC82A6,
		BLIP_MODIFIER_POSSE_ENEMY_OWNED = 0x5E944732,
		BLIP_MODIFIER_POSSE_NEUTRAL = 0x1DF90851,
		BLIP_MODIFIER_POSSE_NEUTRAL_OWNED = 0x617D58CD,
		BLIP_MODIFIER_PULSE_FOREVER = 0xD34253F0,
		BLIP_MODIFIER_RADAR_EDGE_ALWAYS = 0x32850803,
		BLIP_MODIFIER_RADAR_EDGE_NEVER = 0xF366785F,
		BLIP_MODIFIER_SCALE_1 = 0x1DC9C9D4,
		BLIP_MODIFIER_SCALE_2 = 0x2ECDEBDC,
		BLIP_MODIFIER_SHOP_UNAVAILABLE = 0x470BBD53,
		BLIP_MODIFIER_SHOW_HEIGHT = 0x3E605A6D,
		BLIP_MODIFIER_SHRINK_WARNING_1 = 0x2A8907B4,
		BLIP_MODIFIER_SHRINK_WARNING_2 = 0x384BA339,
		BLIP_MODIFIER_TEXT_ONLY = 0xF168692F,
		BLIP_MODIFIER_TOD_DAYTIME_ONLY = 0x2E9B7ACE,
		BLIP_MODIFIER_TOD_NIGHTTIME_ONLY = 0x717A8A2E,
		BLIP_MODIFIER_TOWN_POSSE_MEMBER = 0x40BEFB22,
		BLIP_MODIFIER_TRACKING = 0x287E1591,
		BLIP_MODIFIER_TRAIN_MISSION = 0xE1DE479D,
		BLIP_MODIFIER_UNDISCOVERED = 0x794450C7,
		BLIP_MODIFIER_URGENT = 0xFD364272,
		BLIP_MODIFIER_URGENT_ALERT = 0x229A6F60,
		BLIP_MODIFIER_VERYHIGH_CATEGORY = 0x6A55DFDE,
		BLIP_MODIFIER_WANTED_PULSE_1 = 0x76674069,
		BLIP_MODIFIER_WANTED_PULSE_2 = 0x8CC46D23,
		BLIP_MODIFIER_WANTED_PULSE_3 = 0x1EC09125,
		BLIP_MODIFIER_WANTED_PULSE_4 = 0x3483BCAB,
		BLIP_MODIFIER_WANTED_PULSE_5 = 0x266D86E,
		BLIP_MODIFIER_WITNESS_IDENTIFIED = 0x190F3B7C,
		BLIP_MODIFIER_WITNESS_INVESTIGATING = 0x5E176D3A,
		BLIP_MODIFIER_WITNESS_UNIDENTIFIED = 0xFAA28257,
	}
	public enum BlipType : uint
	{
		CompanionGray = 0x19365607,
		PickupWhite = 0xEC972124,
		WeaponWhite = 0x63351D54,
		WhiteDot = 0xB04092F8,
		Flashing = 0x4B1C3939,
		EnemyPink = 0x9A7FB0BF,
		DestinationSmall = 0xC19DA63,
		DestinationGray = 0xD792CF71,
		PosseCamp = 0x5D0509CC,
		DestinationLarge = 0x1857A152,
		DestinationDark = 0x64994D7C,
        BLIP_STYLE_ACCURATE_AREA_BOUNDS_OVERLAY = 0xF4CB3AA9,
		BLIP_STYLE_ACCURATE_AREA_BOUNDS_OVERLAY_WARNING = 0xE762FC29,
		BLIP_STYLE_ADVERSARY = 0x83A81FBC,
		BLIP_STYLE_AMBIENT_COACH = 0xAC524F01,
		BLIP_STYLE_AREA = 0x4A60B7C0,
		BLIP_STYLE_AREA_BOUNDS = 0xEEF34897,
		BLIP_STYLE_AREA_BOUNDS_OVERLAY = 0xDE0E8279,
		BLIP_STYLE_AREA_BOUNDS_OVERLAY_BR_GRID = 0xE9E24F6C,
		BLIP_STYLE_AREA_BOUNDS_OVERLAY_BR_GRID_WARNING = 0x6C273786,
		BLIP_STYLE_AREA_BOUNDS_OVERLAY_WARNING = 0x773DF41,
		BLIP_STYLE_AREA_BURN = 0x2AE9F922,
		BLIP_STYLE_AREA_DIRECTIONAL = 0xA146C369,
		BLIP_STYLE_AREA_DIRECTIONAL_HEIGHT = 0x1DA4D9C8,
		BLIP_STYLE_AREA_FOW = 0x7932E07C,
		BLIP_STYLE_AREA_NO_DIM = 0x24FD8E7C,
		BLIP_STYLE_AREA_TERRITORY_TRAIN = 0x67EBAE7B,
		BLIP_STYLE_AVOID_AREA = 0x41156724,
		BLIP_STYLE_AVOID_RADIUS = 0xD84734FB,
		BLIP_STYLE_BOUNTY_HUNTER = 0x1F242DF7,
		BLIP_STYLE_BOUNTY_HUNTER_BASE = 0x718ECD0,
		BLIP_STYLE_BOUNTY_HUNTER_RADIUS = 0xD1001702,
		BLIP_STYLE_BOUNTY_TARGET = 0x38CDE89D,
		BLIP_STYLE_BOUNTY_TARGET_INCAPACITATED = 0x1B294DDA,
		BLIP_STYLE_CAMP = 0x5D0509CC,
		BLIP_STYLE_CAMP_CHORE = 0xD7C61725,
		BLIP_STYLE_CAMP_ENEMY = 0x8B708437,
		BLIP_STYLE_CAMP_HIDDEN = 0x6686C0B1,
		BLIP_STYLE_CAMP_REQUEST = 0x8C15671C,
		BLIP_STYLE_CAMPFIRE = 0xEBF814FD,
		BLIP_STYLE_CAR = 0xAC6A6B7B,
		BLIP_STYLE_CENTER = 0x3EA0A440,
		BLIP_STYLE_CHALLENGE_OBJECTIVE = 0xBF7C6760,
		BLIP_STYLE_CHECKPOINT_CHALLENGE = 0x780B624E,
		BLIP_STYLE_COP = 0xA0ED772A,
		BLIP_STYLE_COP_BASE = 0x2782D584,
		BLIP_STYLE_COP_DOG = 0xFEBA085F,
		BLIP_STYLE_COP_DOG_BASE = 0x28320506,
		BLIP_STYLE_COP_PERSISTENT = 0xE793A75A,
		BLIP_STYLE_COP_SCRIPT = 0xF8F74DCE,
		BLIP_STYLE_CREATOR_CURSOR = 0x4418BF91,
		BLIP_STYLE_CREATOR_DEFAULT = 0xB8D5DE4E,
		BLIP_STYLE_CREATOR_PROP = 0x443B0E82,
		BLIP_STYLE_DEBUG_BLUE = 0x29BD62ED,
		BLIP_STYLE_DEBUG_GREEN = 0xA68C5F42,
		BLIP_STYLE_DEBUG_PINKK = 0xA9D8416F,
		BLIP_STYLE_DEBUG_RED = 0x66C728B8,
		BLIP_STYLE_DEBUG_YELLOW = 0xA0F3C9B8,
		BLIP_STYLE_DEPUTY_RESIDENT = 0x185CC053,
		BLIP_STYLE_DEPUTY_RESIDENT_BASE = 0x3EE8F917,
		BLIP_STYLE_DISTRICT_HEARTLANDS = 0x804E7235,
		BLIP_STYLE_ENEMY = 0x318C617C,
		BLIP_STYLE_ENEMY_NO_THREAT = 0xCDF83C77,
		BLIP_STYLE_ENEMY_PASSIVE_THREAT = 0xF4E49D69,
		BLIP_STYLE_ENEMY_SEVERE = 0x12BD604C,
		BLIP_STYLE_EVADE_AREA = 0x69CA528D,
		BLIP_STYLE_EVENT_AREA = 0xE27F3ACC,
		BLIP_STYLE_EYEWITNESS = 0xC17393B9,
		BLIP_STYLE_EYEWITNESS_NO_CONDITION = 0xC3CDAD74,
		BLIP_STYLE_FAKE_WANTED_RADIUS = 0x294EE1FD,
		BLIP_STYLE_FETCH_TRAIN = 0xFB2F2212,
		BLIP_STYLE_FIREWOOD_BLIP = 0x9F684D6E,
		BLIP_STYLE_FISHING_SPOT = 0xA9A39272,
		BLIP_STYLE_FM_EVENT = 0x24E43740,
		BLIP_STYLE_FM_EVENT_AREA = 0xAAACD360,
		BLIP_STYLE_FM_EVENT_COACH = 0x7B1400AA,
		BLIP_STYLE_FM_EVENT_RADIUS = 0xFD2B385B,
		BLIP_STYLE_FREE_CAMERA = 0xF5B011D4,
		BLIP_STYLE_FRIENDLY = 0xFAAB68A9,
		BLIP_STYLE_FRIENDLY_ON_RADAR = 0x361638E5,
		BLIP_STYLE_GANG_BOUNTY_RADIUS = 0xE8B6424F,
		BLIP_STYLE_GANG_WAYPOINT = 0xC0453A9D,
		BLIP_STYLE_GOLDEN_HAT = 0x5470F88C,
		BLIP_STYLE_HERDING_ANIMAL = 0xD638AC3C,
		BLIP_STYLE_HERDING_DOG = 0x5DB859EA,
		BLIP_STYLE_HERDING_MAIN = 0x95B3973D,
		BLIP_STYLE_HERDING_STRAGGLER = 0xF33C2519,
		BLIP_STYLE_HITCHING_POST = 0x64994D7C,
		BLIP_STYLE_LEGENDARY_ANIMAL_AREA = 0xF985FFB8,
		BLIP_STYLE_LEGENDARY_ANIMAL_CLUE = 0x5BAE6134,
		BLIP_STYLE_LOOT_OBJECTIVE = 0x3EE98B8E,
		BLIP_STYLE_MINIGAME = 0xD792CF71,
		BLIP_STYLE_MISSION = 0x63B83205,
		BLIP_STYLE_MISSION_HIDDEN = 0xDCABFD68,
		BLIP_STYLE_MISSION_RADAR_ONLY = 0x9C7E313,
		BLIP_STYLE_MISSION_RADIUS = 0xA305B416,
		BLIP_STYLE_MISSION_RADIUS_HIDDEN = 0x766A98A5,
		BLIP_STYLE_MP_CAMP_FOLLOWER = 0x32A5E2DF,
		BLIP_STYLE_MP_CORPSE_FADE = 0x6FE941A2,
		BLIP_STYLE_MP_CORPSE_FADE_LOCAL = 0x5F0FFE1,
		BLIP_STYLE_MP_DOWNED = 0x7A5D7368,
		BLIP_STYLE_MP_HIDEOUT = 0xE1C50372,
		BLIP_STYLE_MP_JOB_PLAYER_ALLY = 0xEF8D30A6,
		BLIP_STYLE_MP_JOB_PLAYER_ENEMY = 0x32FDCE37,
		BLIP_STYLE_MP_JOB_PLAYER_ENEMY_ANIMATE_ON_ATTACK = 0x8F1B18B4,
		BLIP_STYLE_MP_JOB_PLAYER_ENEMY_ANIMATE_ON_ATTACK_FTB = 0xEAAF4AEC,
		BLIP_STYLE_MP_JOB_PLAYER_ENEMY_NO_THREAT = 0xD433F7EB,
		BLIP_STYLE_MP_MISSION_GIVER = 0x654604CD,
		BLIP_STYLE_MP_PLAYER = 0x69DAFC52,
		BLIP_STYLE_MP_PLAYLIST_CORONA = 0xA325AAD8,
		BLIP_STYLE_MP_SCHEDULE_SERIES = 0x5805D4B4,
		BLIP_STYLE_MP_SERIES = 0xEB521B3B,
		BLIP_STYLE_NEUTRAL = 0x97B6F06C,
		BLIP_STYLE_NEUTRAL_OBJECTIVE = 0x61394695,
		BLIP_STYLE_OBJECTIVE_MINOR = 0xC19DA63,
		BLIP_STYLE_OBJECTIVE_VOL_BOX = 0x4B0BA4D4,
		BLIP_STYLE_OBJECTIVE_VOL_ROUND = 0xED9B5A10,
		BLIP_STYLE_OTHER_SESSION_PLAYER = 0x4EFEAB8A,
		BLIP_STYLE_PICKUP = 0xEC972124,
		BLIP_STYLE_PICKUP_AMMO = 0x7379C999,
		BLIP_STYLE_PICKUP_ANIMAL = 0xA0F8DA2D,
		BLIP_STYLE_PICKUP_ANIMAL_DOWNED = 0xE9949714,
		BLIP_STYLE_PICKUP_ANIMAL_DOWNED_ENEMY = 0xCF595F1A,
		BLIP_STYLE_PICKUP_ANIMAL_SKIN = 0xB5FC8942,
		BLIP_STYLE_PICKUP_COLLECTABLE = 0x75950C3,
		BLIP_STYLE_PICKUP_HERB = 0x54F33CD9,
		BLIP_STYLE_PICKUP_HERB_NEW = 0x3BBC40DE,
		BLIP_STYLE_PICKUP_HORSE = 0xA398BA89,
		BLIP_STYLE_PICKUP_OBJECT = 0x7D199FA6,
		BLIP_STYLE_PICKUP_VEHICLE = 0x89D41596,
		BLIP_STYLE_PICKUP_WEAPON = 0x63351D54,
		BLIP_STYLE_PICKUP_WEAPON_DIRECTIONAL = 0x44CF39AF,
		BLIP_STYLE_PICKUP_WILDERNESS_CHEST = 0x5E846C70,
		BLIP_STYLE_PING_AUTO = 0xB1F69C74,
		BLIP_STYLE_PING_BEAT_EXPLOSION = 0xB456DFE8,
		BLIP_STYLE_PING_BELL_RING = 0x5BB356FE,
		BLIP_STYLE_PING_FAST = 0x2AF985E8,
		BLIP_STYLE_PING_FAST_LARGE = 0x603DA5E0,
		BLIP_STYLE_PING_LEGENDARY_ANIMAL_CLUE = 0xF34063C9,
		BLIP_STYLE_PING_MEDIUM = 0x691306EB,
		BLIP_STYLE_PING_MEDIUM_BEAT = 0xD1A67C50,
		BLIP_STYLE_PING_MEDIUM_LONG = 0x106855E7,
		BLIP_STYLE_PING_MEDIUM_NO_BLIP = 0xAF9781C9,
		BLIP_STYLE_PING_MEDIUM_SLOW = 0x5E66117D,
		BLIP_STYLE_PING_MEDIUM_SMALL = 0x4C3C857E,
		BLIP_STYLE_PING_MEDIUM_THREAT = 0x25C64EEF,
		BLIP_STYLE_PING_SLOW = 0x3212A764,
		BLIP_STYLE_PING_TREE_FALL = 0x7067641D,
		BLIP_STYLE_PLANT_DYNAMITE = 0xCE5CAAB0,
		BLIP_STYLE_PLAYER_COACH = 0x25AB0484,
		BLIP_STYLE_PLAYER_HAT_BLIP = 0x1D053A85,
		BLIP_STYLE_PLAYER_HORSE = 0x8D39991C,
		BLIP_STYLE_PLAYER_HORSE_SADDLE = 0xE145D117,
		BLIP_STYLE_POI = 0x9D6AB6CB,
		BLIP_STYLE_POSSE_FORMATION_AREA = 0x945B1A27,
		BLIP_STYLE_PROC_HOME_ROBBERY = 0xDA273FD0,
		BLIP_STYLE_PROC_HOME_ROBBERY_ENEMY = 0x995A0B3C,
		BLIP_STYLE_PROC_MISSION = 0xC83277C6,
		BLIP_STYLE_PROC_MISSION_INSTANCED = 0x24352465,
		BLIP_STYLE_PROC_MISSION_RADIUS = 0x88B1C1CB,
		BLIP_STYLE_PROPERTY = 0xC601B611,
		BLIP_STYLE_PROPERTY_OWNER = 0xDC33A0AD,
		BLIP_STYLE_RADIUS = 0xB38A23C0,
		BLIP_STYLE_RANDOM_EVENT_PULSE = 0x4CF6356D,
		BLIP_STYLE_RCM = 0xB04092F8,
		BLIP_STYLE_RCM_AREA = 0xD9028B4,
		BLIP_STYLE_RCM_BOUNTY = 0xA7794D6D,
		BLIP_STYLE_RCM_BOUNTY_HIDDEN = 0xD10C46,
		BLIP_STYLE_RCM_HIDDEN = 0x23157B57,
		BLIP_STYLE_RCM_TRACKED = 0x7D604B7D,
		BLIP_STYLE_RCM_TRACKED_HIDDEN = 0xC26EBA0C,
		BLIP_STYLE_REGION_LOCKDOWN = 0xB232154F,
		BLIP_STYLE_SELECTED_REGION = 0xE15AFF64,
		BLIP_STYLE_SHOP = 0xA04E692,
		BLIP_STYLE_SP_CORPSE = 0x3AB68EC,
		BLIP_STYLE_SP_CORPSE_FADE = 0x26558465,
		BLIP_STYLE_TEMPORARY_HORSE = 0xB6A087F3,
		BLIP_STYLE_TOWN = 0xE92142B5,
		BLIP_STYLE_TRACKING = 0xBA0E9988,
		BLIP_STYLE_TRACKING_DOT = 0x85D86FCB,
		BLIP_STYLE_TRAIN = 0xE8302B3F,
		BLIP_STYLE_TRAIN_BOOKMARK = 0x563C0A67,
		BLIP_STYLE_TURRET_WEAPON = 0x4C0EF147,
		BLIP_STYLE_UNDISCOVERED = 0xD2D71097,
		BLIP_STYLE_UPCOMING_OBJECTIVE = 0x289A47CD,
		BLIP_STYLE_WANTED_RADIUS = 0x45757068,
		BLIP_STYLE_WANTED_STATE = 0x7CB9547E,
		BLIP_STYLE_WAYPOINT = 0x85C4987B,
	}}
