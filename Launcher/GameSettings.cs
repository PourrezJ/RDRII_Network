using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Launcher
{
    public class GameSettings
    {
        public static Settings LoadGameSettings()
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments,
                Environment.SpecialFolderOption.Create) + "\\Rockstar Games\\Red Dead Redemption 2\\Settings\\system.xml";
            if (!File.Exists(filePath)) return new Settings();

            using (var stream = File.OpenRead(filePath))
            {
                var ser = new XmlSerializer(typeof(Settings));
                var settings = (Settings)ser.Deserialize(stream);
                return settings;
            }
        }

        public static void SaveSettings(Settings sets)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments,
                Environment.SpecialFolderOption.Create) + "\\Rockstar Games\\Red Dead Redemption 2\\Settings\\system.xml";
            using (var stream = new FileStream(filePath, File.Exists(filePath) ? FileMode.Truncate : FileMode.CreateNew)
                )
            {
                var ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(stream, sets);
            }
        }


		[XmlRoot(ElementName = "version")]
		public class Version
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "anisotropicFiltering")]
		public class AnisotropicFiltering
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "fxaaEnabled")]
		public class FxaaEnabled
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "msaa")]
		public class Msaa
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "graphicsQualityPreset")]
		public class GraphicsQualityPreset
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "graphics")]
		public class Graphics
		{
			[XmlElement(ElementName = "tessellation")]
			public string Tessellation { get; set; }
			[XmlElement(ElementName = "shadowQuality")]
			public string ShadowQuality { get; set; }
			[XmlElement(ElementName = "farShadowQuality")]
			public string FarShadowQuality { get; set; }
			[XmlElement(ElementName = "reflectionQuality")]
			public string ReflectionQuality { get; set; }
			[XmlElement(ElementName = "mirrorQuality")]
			public string MirrorQuality { get; set; }
			[XmlElement(ElementName = "ssao")]
			public string Ssao { get; set; }
			[XmlElement(ElementName = "textureQuality")]
			public string TextureQuality { get; set; }
			[XmlElement(ElementName = "particleQuality")]
			public string ParticleQuality { get; set; }
			[XmlElement(ElementName = "waterQuality")]
			public string WaterQuality { get; set; }
			[XmlElement(ElementName = "volumetricsQuality")]
			public string VolumetricsQuality { get; set; }
			[XmlElement(ElementName = "lightingQuality")]
			public string LightingQuality { get; set; }
			[XmlElement(ElementName = "ambientLightingQuality")]
			public string AmbientLightingQuality { get; set; }
			[XmlElement(ElementName = "anisotropicFiltering")]
			public AnisotropicFiltering AnisotropicFiltering { get; set; }
			[XmlElement(ElementName = "taa")]
			public string Taa { get; set; }
			[XmlElement(ElementName = "fxaaEnabled")]
			public FxaaEnabled FxaaEnabled { get; set; }
			[XmlElement(ElementName = "msaa")]
			public Msaa Msaa { get; set; }
			[XmlElement(ElementName = "graphicsQualityPreset")]
			public GraphicsQualityPreset GraphicsQualityPreset { get; set; }
		}

		[XmlRoot(ElementName = "locked")]
		public class Locked
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "asyncComputeEnabled")]
		public class AsyncComputeEnabled
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "transferQueuesEnabled")]
		public class TransferQueuesEnabled
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "motionBlur")]
		public class MotionBlur
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "motionBlurLimit")]
		public class MotionBlurLimit
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "waterReflectionSSR")]
		public class WaterReflectionSSR
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "waterSimulationQuality")]
		public class WaterSimulationQuality
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "maxTexUpgradesPerFrame")]
		public class MaxTexUpgradesPerFrame
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "shadowParticleShadows")]
		public class ShadowParticleShadows
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "shadowLongShadows")]
		public class ShadowLongShadows
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "directionalShadowsAlpha")]
		public class DirectionalShadowsAlpha
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "worldHeightShadowQuality")]
		public class WorldHeightShadowQuality
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "directionalScreenSpaceShadowQuality")]
		public class DirectionalScreenSpaceShadowQuality
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "ambientMaskVolumesHighPrecision")]
		public class AmbientMaskVolumesHighPrecision
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "volumetricsRaymarchResolutionUnclamped")]
		public class VolumetricsRaymarchResolutionUnclamped
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "damageModelsDisabled")]
		public class DamageModelsDisabled
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "ssaoFullScreenEnabled")]
		public class SsaoFullScreenEnabled
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "ssaoType")]
		public class SsaoType
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "ssdoSampleCount")]
		public class SsdoSampleCount
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "ssdoUseDualRadii")]
		public class SsdoUseDualRadii
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "ssdoTAABlendEnabled")]
		public class SsdoTAABlendEnabled
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "ssroSampleCount")]
		public class SsroSampleCount
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "snowGlints")]
		public class SnowGlints
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "probeRelightEveryFrame")]
		public class ProbeRelightEveryFrame
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "reflectionMSAA")]
		public class ReflectionMSAA
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "lodScale")]
		public class LodScale
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "grassLod")]
		public class GrassLod
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "pedLodBias")]
		public class PedLodBias
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "vehicleLodBias")]
		public class VehicleLodBias
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "sharpenIntensity")]
		public class SharpenIntensity
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "treeTessellationEnabled")]
		public class TreeTessellationEnabled
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "advancedGraphics")]
		public class AdvancedGraphics
		{
			[XmlElement(ElementName = "API")]
			public string API { get; set; }
			[XmlElement(ElementName = "locked")]
			public Locked Locked { get; set; }
			[XmlElement(ElementName = "asyncComputeEnabled")]
			public AsyncComputeEnabled AsyncComputeEnabled { get; set; }
			[XmlElement(ElementName = "transferQueuesEnabled")]
			public TransferQueuesEnabled TransferQueuesEnabled { get; set; }
			[XmlElement(ElementName = "shadowSoftShadows")]
			public string ShadowSoftShadows { get; set; }
			[XmlElement(ElementName = "motionBlur")]
			public MotionBlur MotionBlur { get; set; }
			[XmlElement(ElementName = "motionBlurLimit")]
			public MotionBlurLimit MotionBlurLimit { get; set; }
			[XmlElement(ElementName = "particleLightingQuality")]
			public string ParticleLightingQuality { get; set; }
			[XmlElement(ElementName = "waterReflectionSSR")]
			public WaterReflectionSSR WaterReflectionSSR { get; set; }
			[XmlElement(ElementName = "waterRefractionQuality")]
			public string WaterRefractionQuality { get; set; }
			[XmlElement(ElementName = "waterReflectionQuality")]
			public string WaterReflectionQuality { get; set; }
			[XmlElement(ElementName = "waterSimulationQuality")]
			public WaterSimulationQuality WaterSimulationQuality { get; set; }
			[XmlElement(ElementName = "waterLightingQuality")]
			public string WaterLightingQuality { get; set; }
			[XmlElement(ElementName = "furDisplayQuality")]
			public string FurDisplayQuality { get; set; }
			[XmlElement(ElementName = "maxTexUpgradesPerFrame")]
			public MaxTexUpgradesPerFrame MaxTexUpgradesPerFrame { get; set; }
			[XmlElement(ElementName = "shadowGrassShadows")]
			public string ShadowGrassShadows { get; set; }
			[XmlElement(ElementName = "shadowParticleShadows")]
			public ShadowParticleShadows ShadowParticleShadows { get; set; }
			[XmlElement(ElementName = "shadowLongShadows")]
			public ShadowLongShadows ShadowLongShadows { get; set; }
			[XmlElement(ElementName = "directionalShadowsAlpha")]
			public DirectionalShadowsAlpha DirectionalShadowsAlpha { get; set; }
			[XmlElement(ElementName = "worldHeightShadowQuality")]
			public WorldHeightShadowQuality WorldHeightShadowQuality { get; set; }
			[XmlElement(ElementName = "directionalScreenSpaceShadowQuality")]
			public DirectionalScreenSpaceShadowQuality DirectionalScreenSpaceShadowQuality { get; set; }
			[XmlElement(ElementName = "ambientMaskVolumesHighPrecision")]
			public AmbientMaskVolumesHighPrecision AmbientMaskVolumesHighPrecision { get; set; }
			[XmlElement(ElementName = "scatteringVolumeQuality")]
			public string ScatteringVolumeQuality { get; set; }
			[XmlElement(ElementName = "volumetricsRaymarchQuality")]
			public string VolumetricsRaymarchQuality { get; set; }
			[XmlElement(ElementName = "volumetricsLightingQuality")]
			public string VolumetricsLightingQuality { get; set; }
			[XmlElement(ElementName = "volumetricsRaymarchResolutionUnclamped")]
			public VolumetricsRaymarchResolutionUnclamped VolumetricsRaymarchResolutionUnclamped { get; set; }
			[XmlElement(ElementName = "terrainShadowQuality")]
			public string TerrainShadowQuality { get; set; }
			[XmlElement(ElementName = "damageModelsDisabled")]
			public DamageModelsDisabled DamageModelsDisabled { get; set; }
			[XmlElement(ElementName = "decalQuality")]
			public string DecalQuality { get; set; }
			[XmlElement(ElementName = "ssaoFullScreenEnabled")]
			public SsaoFullScreenEnabled SsaoFullScreenEnabled { get; set; }
			[XmlElement(ElementName = "ssaoType")]
			public SsaoType SsaoType { get; set; }
			[XmlElement(ElementName = "ssdoSampleCount")]
			public SsdoSampleCount SsdoSampleCount { get; set; }
			[XmlElement(ElementName = "ssdoUseDualRadii")]
			public SsdoUseDualRadii SsdoUseDualRadii { get; set; }
			[XmlElement(ElementName = "ssdoResolution")]
			public string SsdoResolution { get; set; }
			[XmlElement(ElementName = "ssdoTAABlendEnabled")]
			public SsdoTAABlendEnabled SsdoTAABlendEnabled { get; set; }
			[XmlElement(ElementName = "ssroSampleCount")]
			public SsroSampleCount SsroSampleCount { get; set; }
			[XmlElement(ElementName = "snowGlints")]
			public SnowGlints SnowGlints { get; set; }
			[XmlElement(ElementName = "POMQuality")]
			public string POMQuality { get; set; }
			[XmlElement(ElementName = "probeRelightEveryFrame")]
			public ProbeRelightEveryFrame ProbeRelightEveryFrame { get; set; }
			[XmlElement(ElementName = "scalingMode")]
			public string ScalingMode { get; set; }
			[XmlElement(ElementName = "reflectionMSAA")]
			public ReflectionMSAA ReflectionMSAA { get; set; }
			[XmlElement(ElementName = "lodScale")]
			public LodScale LodScale { get; set; }
			[XmlElement(ElementName = "grassLod")]
			public GrassLod GrassLod { get; set; }
			[XmlElement(ElementName = "pedLodBias")]
			public PedLodBias PedLodBias { get; set; }
			[XmlElement(ElementName = "vehicleLodBias")]
			public VehicleLodBias VehicleLodBias { get; set; }
			[XmlElement(ElementName = "sharpenIntensity")]
			public SharpenIntensity SharpenIntensity { get; set; }
			[XmlElement(ElementName = "treeQuality")]
			public string TreeQuality { get; set; }
			[XmlElement(ElementName = "deepsurfaceQuality")]
			public string DeepsurfaceQuality { get; set; }
			[XmlElement(ElementName = "treeTessellationEnabled")]
			public TreeTessellationEnabled TreeTessellationEnabled { get; set; }
		}

		[XmlRoot(ElementName = "adapterIndex")]
		public class AdapterIndex
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "outputIndex")]
		public class OutputIndex
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "resolutionIndex")]
		public class ResolutionIndex
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "screenWidth")]
		public class ScreenWidth
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "screenHeight")]
		public class ScreenHeight
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "refreshRateIndex")]
		public class RefreshRateIndex
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "refreshRateNumerator")]
		public class RefreshRateNumerator
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "refreshRateDenominator")]
		public class RefreshRateDenominator
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "windowed")]
		public class Windowed
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "vSync")]
		public class VSync
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "tripleBuffered")]
		public class TripleBuffered
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "pauseOnFocusLoss")]
		public class PauseOnFocusLoss
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "constrainMousePointer")]
		public class ConstrainMousePointer
		{
			[XmlAttribute(AttributeName = "value")]
			public string Value { get; set; }
		}

		[XmlRoot(ElementName = "video")]
		public class Video
		{
			[XmlElement(ElementName = "adapterIndex")]
			public AdapterIndex AdapterIndex { get; set; }

			[XmlElement(ElementName = "outputIndex")]
			public OutputIndex OutputIndex { get; set; }

			[XmlElement(ElementName = "resolutionIndex")]
			public ResolutionIndex ResolutionIndex { get; set; }

			[XmlElement(ElementName = "screenWidth")]
			public ScreenWidth ScreenWidth { get; set; }

			[XmlElement(ElementName = "screenHeight")]
			public ScreenHeight ScreenHeight { get; set; }

			[XmlElement(ElementName = "refreshRateIndex")]
			public RefreshRateIndex RefreshRateIndex { get; set; }

			[XmlElement(ElementName = "refreshRateNumerator")]
			public RefreshRateNumerator RefreshRateNumerator { get; set; }

			[XmlElement(ElementName = "refreshRateDenominator")]
			public RefreshRateDenominator RefreshRateDenominator { get; set; }

			[XmlElement(ElementName = "windowed")]
			public Windowed Windowed { get; set; }

			[XmlElement(ElementName = "vSync")]
			public VSync VSync { get; set; }

			[XmlElement(ElementName = "tripleBuffered")]
			public TripleBuffered TripleBuffered { get; set; }

			[XmlElement(ElementName = "pauseOnFocusLoss")]
			public PauseOnFocusLoss PauseOnFocusLoss { get; set; }

			[XmlElement(ElementName = "constrainMousePointer")]
			public ConstrainMousePointer ConstrainMousePointer { get; set; }
		}

		[XmlRoot(ElementName = "rage__fwuiSystemSettingsCollection")]
		public class Settings
		{
			[XmlElement(ElementName = "version")]
			public Version Version { get; set; }
			[XmlElement(ElementName = "configSource")]
			public string ConfigSource { get; set; }
			[XmlElement(ElementName = "graphics")]
			public Graphics Graphics { get; set; }
			[XmlElement(ElementName = "advancedGraphics")]
			public AdvancedGraphics AdvancedGraphics { get; set; }
			[XmlElement(ElementName = "video")]
			public Video Video { get; set; }
			[XmlElement(ElementName = "videoCardDescription")]
			public string VideoCardDescription { get; set; }

		}
	}
}