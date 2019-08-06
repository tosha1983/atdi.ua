﻿using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class ReferenceSpectrum
    {
        public static float[] GSMLevels = new float [1638] { -92.17799377f, -92.96498108f, -93.52687073f, -92.9032135f, -91.99375153f, -91.73587036f, -91.68135834f, -91.56856537f, -93.35743713f, -96.12982941f, -97.88240814f, -98.33181763f, -97.78511047f, -96.21208191f, -94.63594055f, -93.32268524f, -92.85223389f, -93.57206726f, -94.68865204f, -93.65680695f, -91.60784912f, -90.34134674f, -90.19599915f, -91.71051788f, -95.10914612f, -96.53701782f, -93.69580841f, -92.24919891f, -92.50123596f, -93.17696381f, -92.46060944f, -91.22817993f, -90.43340302f, -89.96920013f, -90.15781403f, -90.05172729f, -90.35547638f, -90.18103027f, -89.67202759f, -90.91311646f, -91.7157135f, -89.02594757f, -87.71615601f, -87.35678864f, -88.221138f, -88.79268646f, -90.66527557f, -90.31492615f, -90.91608429f, -92.82932281f, -93.10767365f, -93.15480804f, -93.80165863f, -92.70206451f, -90.76415253f, -89.55632782f, -88.66310883f, -88.44907379f, -89.11846924f, -89.80101776f, -89.97063446f, -89.90718842f, -89.99699402f, -91.57401276f, -94.47544861f, -93.63236237f, -91.27217865f, -91.57010651f, -92.59088898f, -93.52754211f, -92.49725342f, -90.62493134f, -90.86128235f, -90.52524567f, -90.22977448f, -90.98880005f, -90.39865112f, -89.55995941f, -89.23143768f, -89.70671844f, -90.67092133f, -91.57704163f, -93.31065369f, -92.4802475f, -90.37389374f, -90.83574677f, -93.16677094f, -94.5136795f, -89.84471893f, -88.30883789f, -88.52215576f, -89.35870361f, -89.84719849f, -89.69288635f, -89.686409f, -90.06509399f, -90.53831482f, -91.13697815f, -91.19178772f, -89.4675293f, -87.93383026f, -87.70628357f, -88.36122894f, -89.03375244f, -89.88961029f, -90.88172913f, -91.15753174f, -90.88685608f, -89.95787811f, -89.55631256f, -88.66532135f, -88.16087341f, -88.15658569f, -89.26689911f, -91.04005432f, -90.27619934f, -89.25835419f, -88.99784851f, -89.4489212f, -89.33041382f, -89.05460358f, -88.79553223f, -88.64822388f, -88.48031616f, -88.62849426f, -90.0716095f, -90.5334549f, -90.65099335f, -90.25983429f, -90.03501129f, -92.02325439f, -91.46547699f, -89.52628326f, -89.929245f, -90.97160339f, -91.42581177f, -90.36405945f, -88.70993805f, -85.652771f, -84.92099762f, -85.41682434f, -87.28240204f, -91.76715088f, -92.17533875f, -90.78813934f, -89.74420929f, -89.39812469f, -89.86890411f, -90.41155243f, -89.90592194f, -89.42654419f, -89.37493896f, -89.34632874f, -90.94283295f, -94.83586884f, -94.85688019f, -97.35350037f, -96.33969116f, -93.60523224f, -91.86734009f, -91.57743073f, -91.68860626f, -91.68149567f, -91.78670502f, -92.13499451f, -92.6398468f, -91.5117569f, -89.57077026f, -87.97396851f, -86.98503876f, -88.44268036f, -91.74385071f, -92.49420166f, -90.19054413f, -88.84822083f, -88.90695953f, -90.34317017f, -91.43409729f, -91.28982544f, -91.36807251f, -91.5528183f, -92.13048553f, -93.01434326f, -93.89852905f, -93.56098175f, -94.16082001f, -93.42816162f, -92.5896225f, -91.72969055f, -91.61531067f, -92.41542053f, -93.28590393f, -94.62805939f, -94.86906433f, -93.89840698f, -91.99663544f, -90.99417877f, -92.14315796f, -94.34960175f, -95.08804321f, -91.9353714f, -89.79846954f, -88.90699005f, -88.95087433f, -90.16448212f, -91.85005188f, -91.36318207f, -89.7831955f, -89.19773865f, -90.02734375f, -92.28881073f, -94.30336761f, -93.51473236f, -92.86865234f, -93.27936554f, -92.86319733f, -91.85203552f, -90.58764648f, -90.18713379f, -90.31697845f, -90.84146118f, -90.89753723f, -90.88452148f, -91.10218811f, -90.3282547f, -89.11026001f, -88.55545807f, -88.69350433f, -89.19702148f, -88.74886322f, -87.44820404f, -88.60987854f, -89.57836914f, -88.96620178f, -87.32238007f, -85.49407959f, -84.7766571f, -85.04386902f, -86.03691101f, -86.80384827f, -86.45960236f, -85.22098541f, -84.29238129f, -84.43017578f, -85.31840515f, -86.55810547f, -88.15699768f, -87.68021393f, -86.57315063f, -87.23529816f, -88.49388885f, -87.33138275f, -85.43375397f, -83.48365021f, -82.57397461f, -82.72687531f, -82.74847412f, -81.94485474f, -81.8565979f, -82.44281006f, -82.82183838f, -83.28163147f, -83.52816772f, -81.86877441f, -80.92455292f, -80.20889282f, -80.21642303f, -80.72141266f, -80.26238251f, -79.80496979f, -79.34781647f, -79.32057953f, -79.51603699f, -80.358078f, -80.89841461f, -79.3398056f, -78.40763092f, -78.52533722f, -78.61342621f, -78.1466217f, -77.94702148f, -77.79093933f, -78.17536926f, -80.97038269f, -84.34444427f, -83.21019745f, -81.40726471f, -79.75205231f, -79.01529694f, -79.28793335f, -79.55472565f, -79.24538422f, -78.16293335f, -77.52928162f, -78.32397461f, -78.93395996f, -80.06511688f, -80.75930023f, -79.78610229f, -78.62340546f, -77.32460785f, -76.86880493f, -76.95176697f, -77.44642639f, -78.99567413f, -79.82723236f, -80.10016632f, -79.39363861f, -77.84454346f, -76.080513f, -74.83695984f, -74.71215057f, -76.1071167f, -77.46205139f, -76.20163727f, -76.05913544f, -76.67216492f, -76.48613739f, -75.58068848f, -74.56781769f, -73.99697876f, -73.60084534f, -73.51055145f, -73.69101715f, -74.41291809f, -75.04959869f, -73.79633331f, -73.79802704f, -74.19696045f, -74.54728699f, -74.57595062f, -74.03814697f, -73.55666351f, -73.74938965f, -74.2535553f, -76.28945923f, -80.76303101f, -76.76351166f, -71.17281342f, -70.19897461f, -71.41436768f, -72.54087067f, -73.19054413f, -73.82662964f, -73.68378448f, -72.81189728f, -72.44935608f, -72.34886169f, -73.48711395f, -73.93231964f, -73.10006714f, -72.65759277f, -73.04988098f, -73.22058868f, -72.27148438f, -71.45676422f, -72.26750183f, -73.45407104f, -73.09150696f, -72.92016602f, -71.58026123f, -69.89258575f, -68.96731567f, -69.49319458f, -69.79151917f, -69.21179199f, -69.62702942f, -71.52881622f, -72.76272583f, -70.64498901f, -69.18318939f, -68.84963989f, -68.8072052f, -69.04266357f, -70.34429932f, -71.92330933f, -69.65956879f, -67.9420929f, -67.7389679f, -67.94724274f, -69.05591583f, -71.00487518f, -71.95652771f, -70.54159546f, -68.58402252f, -67.36331177f, -68.32221222f, -69.79279327f, -69.6964798f, -68.6349411f, -67.32271576f, -67.31832886f, -68.96226501f, -70.22240448f, -67.80872345f, -66.91504669f, -66.66158295f, -67.83048248f, -69.33443451f, -69.35113525f, -69.26099396f, -68.73312378f, -67.26387787f, -66.12886047f, -65.57904053f, -65.25440216f, -65.13879395f, -65.4719162f, -65.74256897f, -66.47370148f, -67.32357025f, -68.20518494f, -68.46641541f, -68.75424957f, -67.73654175f, -66.63861084f, -66.79818726f, -67.64674377f, -67.68161011f, -67.28475189f, -68.89201355f, -68.22842407f, -65.8885498f, -64.21722412f, -63.44670868f, -63.52934265f, -64.26461792f, -64.86858368f, -64.63987732f, -64.25777435f, -64.44981384f, -64.93785858f, -64.47511292f, -64.06403351f, -64.2338562f, -63.52052307f, -63.76997757f, -65.98104858f, -69.04776001f, -65.00828552f, -62.00146103f, -61.94269943f, -61.97335434f, -62.02914429f, -62.61826324f, -63.47654343f, -63.7737236f, -64.25473785f, -65.64502716f, -66.20861816f, -64.8463974f, -63.87500763f, -64.05168152f, -64.67284393f, -64.37714386f, -63.34907532f, -62.92269135f, -64.1524353f, -65.22551727f, -65.88265228f, -67.11354065f, -68.95172119f, -67.2420578f, -64.68854523f, -64.43405914f, -65.59396362f, -64.97885132f, -63.43676758f, -62.17020416f, -64.45611572f, -67.68582916f, -66.72648621f, -63.36690903f, -61.44624329f, -60.31023407f, -60.0663147f, -61.15470886f, -62.39536667f, -63.05900955f, -63.35827637f, -62.43501663f, -61.74596786f, -62.48315048f, -63.38925552f, -62.08476257f, -59.51601028f, -58.10258484f, -58.3024292f, -60.1475296f, -62.40430832f, -62.17403793f, -60.50307846f, -59.18648529f, -59.56378555f, -59.13346863f, -60.52233124f, -61.62680054f, -59.91792679f, -58.88468933f, -59.25589371f, -60.07493591f, -61.39837646f, -63.11406326f, -63.88368607f, -63.23311234f, -62.42837143f, -61.95919418f, -61.07789612f, -59.8402977f, -59.53070831f, -59.74012756f, -61.76609039f, -64.75798798f, -64.57650757f, -64.42223358f, -62.84767532f, -61.75524521f, -60.99489975f, -62.05432892f, -61.07526779f, -59.32220459f, -59.40260315f, -60.87691116f, -64.32505035f, -62.14861679f, -59.9241333f, -61.01399231f, -61.33382416f, -60.14958572f, -59.77508163f, -60.43736649f, -60.57936859f, -61.27556992f, -62.23596954f, -62.11738205f, -61.37481308f, -61.23641586f, -61.47072983f, -61.22865677f, -60.47754288f, -59.23947906f, -57.25934601f, -57.35646057f, -57.96472549f, -58.85288239f, -59.3023262f, -60.11126709f, -60.29511642f, -58.46621323f, -56.87039185f, -57.48823166f, -58.5921402f, -59.07905197f, -57.78930664f, -56.03231049f, -55.67771149f, -56.15948486f, -56.49211884f, -56.16504288f, -55.38269424f, -55.08174896f, -56.13519287f, -58.16096497f, -58.33731461f, -58.67291641f, -57.58824158f, -56.70515442f, -56.81058884f, -58.59130859f, -59.78935242f, -60.44511032f, -59.06626892f, -58.35844421f, -59.17990112f, -59.48000717f, -59.77390289f, -60.67036819f, -62.59594727f, -64.16394043f, -59.32723999f, -57.94327164f, -57.62109756f, -56.11120224f, -54.79074097f, -54.84965897f, -56.29004669f, -58.23589325f, -57.35998917f, -54.48369217f, -53.19125366f, -55.75930405f, -57.37836838f, -56.01095963f, -55.31436539f, -55.61405563f, -57.02090073f, -58.79141998f, -60.09020996f, -58.67790604f, -59.60050964f, -58.3147316f, -55.37418747f, -56.0594902f, -58.30270386f, -56.05797195f, -54.0704422f, -53.65528488f, -54.95453262f, -55.44790649f, -56.3609848f, -56.66572189f, -56.51423264f, -56.24145126f, -56.5030899f, -58.116642f, -59.90674973f, -58.7807045f, -57.70618057f, -58.23339844f, -58.8865242f, -57.56655502f, -56.35316849f, -56.19016266f, -57.07898712f, -58.02827454f, -57.67721939f, -56.71652222f, -56.2091217f, -56.46612549f, -57.15613174f, -56.99025345f, -55.81780624f, -55.2095871f, -55.95048523f, -55.9812355f, -56.38048172f, -57.86668777f, -58.79838562f, -58.60853958f, -57.56160355f, -55.88579941f, -56.61007309f, -58.04266357f, -58.98252869f, -58.35008621f, -54.4376564f, -52.30288696f, -52.67222595f, -54.02661514f, -54.61589813f, -54.79607391f, -54.814785f, -54.71044159f, -53.70800018f, -56.81266403f, -56.7399559f, -54.69500351f, -53.99359131f, -54.29150772f, -55.59580231f, -58.07982254f, -59.86856079f, -59.35850143f, -58.07164383f, -56.63404083f, -59.84633255f, -63.13168335f, -56.70396805f, -54.12633896f, -54.06892395f, -55.62739944f, -57.11211014f, -56.15312195f, -54.47849274f, -53.8849144f, -55.66441345f, -58.71828842f, -57.47343063f, -55.61563873f, -55.09056091f, -55.73837662f, -56.97020721f, -58.26865768f, -59.2099762f, -57.67747879f, -55.25544357f, -54.71497345f, -54.0551033f, -54.01581573f, -53.25322723f, -53.6245575f, -54.8585701f, -56.78952408f, -56.75783539f, -53.99316406f, -53.41869354f, -52.83243561f, -52.34682465f, -53.46112823f, -53.08242035f, -51.92316818f, -51.98823166f, -52.69606781f, -53.68757248f, -54.26590729f, -53.80109024f, -51.65299988f, -49.72797394f, -48.83398056f, -50.77482605f, -53.48349762f, -51.40087128f, -52.75720978f, -53.45176315f, -53.94685745f, -54.991745f, -55.82182693f, -54.57138824f, -53.64451981f, -52.97654724f, -51.71579361f, -51.26888657f, -52.01042938f, -53.415802f, -53.7468071f, -52.80949783f, -52.87563705f, -54.43559265f, -55.21868896f, -56.07811737f, -56.44300842f, -55.10876083f, -53.85319901f, -54.52472305f, -53.37366486f, -52.27891541f, -53.13708496f, -55.03374481f, -55.24234772f, -53.30113983f, -51.76432037f, -51.99397278f, -52.87543488f, -52.63454437f, -51.89722824f, -52.9581337f, -54.13193893f, -55.38980484f, -55.74590302f, -55.11955261f, -53.99685669f, -52.16903305f, -50.5426178f, -49.55554962f, -49.42755508f, -49.72531128f, -51.68166733f, -51.00993729f, -49.68119431f, -49.6291008f, -51.09858704f, -53.55508423f, -53.85650635f, -52.55294418f, -52.3455162f, -53.69455719f, -54.6309166f, -53.8035202f, -52.47032547f, -52.29199219f, -53.34185791f, -55.27416229f, -56.75446701f, -57.35694122f, -56.24715042f, -54.71345901f, -54.35662842f, -53.23585892f, -52.50938034f, -51.32762909f, -50.42849731f, -51.03723145f, -52.71536255f, -53.64651108f, -53.49362946f, -53.27067566f, -53.58526993f, -54.31815338f, -54.70864487f, -55.21503067f, -56.23341751f, -56.25570297f, -55.06351471f, -54.4780159f, -54.33431244f, -55.40945816f, -56.77096939f, -52.6637764f, -50.17479706f, -51.61070251f, -53.64489746f, -54.39990616f, -53.98682404f, -53.34980011f, -53.23460388f, -54.00054932f, -56.25141144f, -57.09824371f, -54.78302002f, -54.20470428f, -54.68708038f, -55.38297272f, -54.86791992f, -54.11922073f, -53.04398727f, -50.41204071f, -49.97979355f, -51.19960785f, -53.10385513f, -54.19371796f, -52.71963882f, -50.6436348f, -50.41080093f, -52.08508682f, -55.61848831f, -58.06774139f, -56.68875885f, -53.6596756f, -51.57214737f, -53.21004105f, -54.37270355f, -51.84708786f, -50.96334076f, -50.92559814f, -52.04335403f, -55.72718811f, -56.91397858f, -53.96351242f, -52.73789978f, -53.10982132f, -54.41584396f, -55.91516495f, -55.04161835f, -52.41992188f, -50.84011841f, -50.6827774f, -52.51101303f, -53.73225403f, -53.42881393f, -51.74342346f, -51.03601074f, -51.54818726f, -53.30200577f, -55.01444626f, -55.67801666f, -54.82465744f, -54.28619766f, -53.93509293f, -52.44240952f, -51.73902512f, -52.16841125f, -52.34208679f, -52.2726059f, -52.17882156f, -55.57389832f, -57.23802185f, -55.99499893f, -55.35830307f, -55.35678101f, -54.91381454f, -53.72514343f, -53.49762344f, -54.28274155f, -54.50638199f, -53.83262634f, -53.48148727f, -53.36774063f, -53.07311249f, -53.33102417f, -52.43202591f, -52.80268478f, -53.51977158f, -52.73445892f, -52.50933075f, -52.88205338f, -52.09508896f, -50.63153839f, -49.75390625f, -49.77501678f, -49.29643631f, -49.72341156f, -51.16863251f, -52.63519287f, -52.70074081f, -51.5701561f, -51.57868576f, -52.6926918f, -55.11200333f, -56.61919785f, -55.06747818f, -53.50050735f, -52.44044876f, -52.21113968f, -53.19324112f, -55.32605362f, -55.01488113f, -52.24245071f, -50.59099579f, -50.55462646f, -52.56897736f, -56.57193756f, -56.58393097f, -53.4382782f, -51.93022156f, -51.25278854f, -50.90494919f, -50.6237793f, -50.54964828f, -50.68429947f, -50.8350563f, -51.16463089f, -51.17944336f, -51.28578186f, -50.9739151f, -51.56920624f, -53.05667496f, -54.11306763f, -52.04697418f, -49.6260643f, -48.4118309f, -49.65665817f, -50.49047089f, -50.49885559f, -51.31020355f, -52.4466629f, -53.48634338f, -54.38985443f, -55.1940918f, -56.30706406f, -54.31775665f, -52.73523712f, -52.88103104f, -52.15598679f, -51.69022369f, -53.0331955f, -53.34435272f, -54.31381607f, -55.04399109f, -55.37506866f, -56.46446991f, -58.56341171f, -57.2218895f, -53.70092773f, -52.68455124f, -53.63373947f, -54.76502609f, -54.54597855f, -53.41241074f, -53.5547142f, -54.90822601f, -53.93592072f, -53.98629761f, -52.52593613f, -51.17818451f, -51.41543961f, -52.60739136f, -54.06974792f, -54.76852798f, -53.9804039f, -54.64741516f, -54.23241425f, -53.98760223f, -55.54700851f, -56.95068741f, -55.41813278f, -54.74817276f, -55.04466248f, -55.46895981f, -54.52894974f, -53.41749954f, -53.09175873f, -52.34738541f, -52.62303925f, -54.10475922f, -55.5044632f, -54.87616348f, -53.59051132f, -53.41245651f, -54.12881088f, -54.99094009f, -55.44097137f, -54.9552269f, -54.135849f, -54.46429443f, -55.61562729f, -55.4805069f, -55.04682159f, -54.40921402f, -53.46723938f, -54.00418472f, -54.53734589f, -52.79532242f, -51.11888885f, -51.51434708f, -52.3050499f, -53.2306366f, -54.09395981f, -54.20072556f, -54.19124603f, -54.34470749f, -54.45640564f, -57.14983368f, -56.60494995f, -57.08362961f, -56.40526962f, -55.19620132f, -54.98629379f, -57.19923782f, -57.39054871f, -58.83685303f, -60.69617844f, -62.22289658f, -58.82146072f, -55.38943481f, -54.18346405f, -53.11192703f, -52.99876404f, -53.90243912f, -54.6407547f, -53.92391968f, -53.34640503f, -52.39250183f, -51.87247086f, -52.72209167f, -53.13923645f, -53.17418289f, -53.06586456f, -53.11952209f, -53.74776459f, -55.29621887f, -56.49774551f, -56.20918274f, -55.45830917f, -55.10565567f, -55.47265625f, -56.06530762f, -55.75910187f, -54.15597916f, -56.1823349f, -56.77902985f, -53.81194305f, -57.05757523f, -58.77736282f, -56.58558655f, -54.71010971f, -53.95648956f, -55.76553726f, -57.55096436f, -58.2012291f, -57.43157578f, -56.80999374f, -56.82696152f, -57.98836517f, -60.48936081f, -61.84209824f, -60.31575775f, -59.38103867f, -58.95668411f, -57.96233749f, -57.37517548f, -58.0958786f, -59.39332199f, -58.94814682f, -57.55321503f, -56.59792709f, -56.25123215f, -55.77682495f, -55.51139069f, -56.4363327f, -57.84999084f, -58.39339066f, -57.17675781f, -55.13851929f, -55.29010391f, -55.95304871f, -55.86090851f, -58.33162689f, -58.65982819f, -58.86555099f, -58.86800003f, -58.30275345f, -57.17285156f, -55.6148262f, -54.77565384f, -55.42869949f, -58.02150345f, -59.60505295f, -60.44704437f, -59.59492874f, -58.57358932f, -58.35970688f, -59.49267197f, -61.49321365f, -61.18889999f, -61.31502151f, -57.87456894f, -58.29637909f, -60.14988327f, -60.20307159f, -59.90353012f, -59.97323227f, -59.61300659f, -59.68412018f, -59.82495499f, -59.64751434f, -58.48114014f, -56.97735214f, -57.72673798f, -58.72133255f, -60.56721115f, -60.06605148f, -59.21532822f, -59.36473465f, -60.05207443f, -61.46170044f, -61.6160202f, -60.83588791f, -58.84247589f, -57.10824585f, -56.75451279f, -57.51680374f, -59.62248611f, -61.07331467f, -62.56697845f, -63.04489517f, -62.96112823f, -63.86727905f, -62.09139252f, -60.98452759f, -59.19076538f, -58.70928574f, -59.3119812f, -61.21602249f, -61.43842697f, -60.46653366f, -59.71309662f, -59.23566818f, -59.30246353f, -60.32835388f, -61.34255981f, -60.31103516f, -58.68397903f, -58.41188431f, -58.25909424f, -59.38312149f, -59.75987244f, -59.35067749f, -59.12244415f, -58.97802353f, -59.67739868f, -60.90969849f, -60.86107635f, -59.49949265f, -60.31424713f, -61.16904831f, -61.04672623f, -60.52151871f, -61.1973381f, -63.18299103f, -64.52923584f, -62.89995193f, -62.04082108f, -61.28471375f, -61.55047989f, -62.59819794f, -63.54547501f, -63.86027908f, -63.42889786f, -64.08817291f, -64.01500702f, -65.01047516f, -66.8270874f, -66.89046478f, -63.41346741f, -60.72439575f, -60.50154495f, -62.0559082f, -63.45960999f, -63.4189415f, -63.45825577f, -64.116539f, -65.65829468f, -68.12947845f, -69.58782196f, -66.91721344f, -63.60309982f, -61.76620865f, -61.56528854f, -63.20522308f, -66.15248108f, -65.93865204f, -64.60542297f, -66.05102539f, -67.23551178f, -66.84181213f, -65.4729538f, -64.42667389f, -63.59857941f, -63.06003571f, -62.98509598f, -63.35776901f, -63.71679688f, -63.29452133f, -62.95339203f, -63.18627548f, -63.61680603f, -64.25640869f, -64.81702423f, -64.92607117f, -65.09392548f, -65.42739868f, -65.80692291f, -68.09300232f, -66.83341217f, -62.72590637f, -61.78367233f, -62.5469017f, -64.3030777f, -67.0391922f, -70.12762451f, -70.5512085f, -69.47820282f, -67.94645691f, -66.81495667f, -66.76753235f, -67.41306305f, -68.68982697f, -68.64566803f, -67.89168549f, -69.29270935f, -69.93998718f, -68.94508362f, -68.53469849f, -68.19565582f, -68.52659607f, -69.60712433f, -70.20894623f, -69.61193085f, -67.9430542f, -67.69384766f, -68.09009552f, -66.48104858f, -66.01417542f, -67.43080139f, -69.62365723f, -68.70149994f, -67.29880524f, -67.43561554f, -68.66823578f, -69.76599121f, -70.71550751f, -71.85289001f, -70.73036194f, -67.22080994f, -67.63064575f, -68.14057159f, -69.0610733f, -70.36093903f, -71.22073364f, -70.49323273f, -69.2824707f, -69.21849823f, -70.6676712f, -70.68693542f, -72.0271759f, -71.18967438f, -69.50421143f, -69.02068329f, -70.0944519f, -72.23445129f, -71.75356293f, -70.21809387f, -69.73503876f, -70.15522766f, -71.12403107f, -71.98625183f, -71.69309235f, -70.00621796f, -68.67017365f, -69.78420258f, -71.19432831f, -69.97612f, -69.31987f, -69.93367004f, -71.2972641f, -73.61076355f, -77.29478455f, -76.83374023f, -74.75968933f, -74.58227539f, -74.46508789f, -73.29872894f, -72.78766632f, -73.32982635f, -73.2532959f, -72.83747864f, -73.62992096f, -73.19948578f, -72.04592133f, -71.76883698f, -71.69837952f, -71.84408569f, -72.82389832f, -73.57527924f, -73.18254852f, -73.26634979f, -73.48514557f, -74.36027527f, -74.98480225f, -74.38210297f, -73.51129913f, -73.55888367f, -74.5734787f, -76.80755615f, -80.55925751f, -79.79180908f, -74.76327515f, -73.07017517f, -72.8736496f, -72.74412537f, -72.95682526f, -73.71261597f, -74.73860931f, -76.26586914f, -78.64736938f, -81.18513489f, -81.15076447f, -79.53177643f, -78.62802887f, -78.7780304f, -78.29654694f, -76.86262512f, -75.92211914f, -76.62013245f, -77.57706451f, -78.14876556f, -79.82702637f, -80.53834534f, -80.16405487f, -78.74211884f, -77.47505188f, -78.51004028f, -78.96762848f, -78.87090302f, -79.1792984f, -80.78411865f, -83.42547607f, -84.63439178f, -82.72040558f, -80.48925781f, -79.31732178f, -79.56876373f, -80.84877777f, -81.31841278f, -80.94794464f, -81.18149567f, -81.56276703f, -81.58488464f, -82.29382324f, -83.0761795f, -83.07516479f, -82.18798828f, -80.8034668f, -80.2771759f, -80.95428467f, -82.08568573f, -82.1346283f, -81.0146637f, -80.05619812f, -80.2639389f, -81.36009216f, -82.73638153f, -83.55901337f, -83.00164032f, -83.13211823f, -83.12586212f, -83.2746582f, -85.204422f, -86.8699646f, -85.72479248f, -84.27639771f, -85.52061462f, -86.79229736f, -86.96995544f, -86.11464691f, -85.51927948f, -85.93232727f, -88.15986633f, -87.92932129f, -87.69103241f, -87.83618927f, -87.48669434f, -88.15007019f, -88.19882965f, -88.0118103f, -88.01066589f, -87.96448517f, -87.04154968f, -86.92673492f, -88.2417984f, -88.88455963f, -87.23535156f, -88.02490997f, -88.63549042f, -89.44748688f, -90.49419403f, -90.06169128f, -90.03921509f, -90.99878693f, -93.20097351f, -95.51825714f, -93.88664246f, -91.53527832f, -90.57415771f, -91.15693665f, -92.75892639f, -93.56832886f, -90.59886169f, -89.99669647f, -88.26000214f, -87.248909f, -88.62242889f, -91.03116608f, -92.98306274f, -90.67147827f, -89.23755646f, -91.31494904f, -92.26743317f, -93.35832214f, -92.39379883f, -92.11531067f, -91.97315979f, -92.77036285f, -93.92045593f, -93.63657379f, -92.73446655f, -92.66797638f, -92.49056244f, -92.03440857f, -92.89761353f, -92.27362061f, -91.53646088f, -91.26916504f, -91.18112183f, -91.87538147f, -93.71739197f, -95.0817337f, -93.62155914f, -93.00598145f, -91.91753387f, -90.80235291f, -90.19246674f, -89.77640533f, -90.0477066f, -91.49609375f, -94.37830353f, -95.34650421f, -93.52146149f, -92.50676727f, -92.66879272f, -91.90713501f, -92.38871002f, -91.12557983f, -88.86714935f, -88.61643982f, -88.41170502f, -88.10965729f, -88.21450043f, -88.60951233f, -87.92237091f, -87.13136292f, -87.62526703f, -88.47589111f, -89.47498322f, -89.92137146f, -90.19733429f, -90.93907166f, -89.79067993f, -91.63967133f, -91.91059875f, -90.65570831f, -88.95153046f, -87.65092468f, -87.13545227f, -87.57313538f, -88.27908325f, -88.0247879f, -87.39759827f, -87.45018005f, -87.96062469f, -90.62695313f, -89.35268402f, -88.91417694f, -89.54308319f, -90.95368958f, -91.93128967f, -91.10590363f, -89.77178192f, -88.68018341f, -87.65029144f, -88.16443634f, -88.49945068f, -89.18602753f, -89.31777954f, -89.60919189f, -92.43161774f, -92.82979584f, -91.29087067f, -90.06882477f, -89.63021851f, -91.85313416f, -95.65161896f, -94.93045044f, -93.63057709f, -91.11138153f, -88.7942276f, -89.43797302f, -89.32122803f, -89.25042725f, -88.86207581f, -88.02301025f, -86.99031067f, -86.65956879f, -87.680336f, -89.01961517f, -88.75131989f, -88.21934509f, -87.52710724f, -87.85836029f, -89.00507355f, -91.95102692f, -91.63776398f, -90.62628937f, -88.97738647f, -88.25478363f, -88.46237183f, -90.65262604f, -90.83651733f, -89.865242f, -90.00070953f, -91.28881073f, -92.8782196f, -92.69924164f, -91.14074707f, -89.9161911f, -89.48266602f, -89.70658875f, -89.50219727f, -88.60553741f, -87.95874023f, -88.21683502f, -89.80744934f, -91.69032288f, -91.90556335f, -91.77927399f, -92.17478943f, -92.6243515f, -92.44552612f, -92.23691559f, -92.93804169f, -93.85704041f, -93.56751251f, -92.77191162f, -92.36352539f, -92.14756012f, -91.40666199f, -90.6659317f, -91.48279572f, -92.40923309f, -92.9659729f, -92.83243561f, -92.37669373f, -91.31581879f, -89.9942627f, -91.34487915f, -90.63869476f, -89.81138611f, -89.84931946f, -89.55027008f, -89.43593597f, -89.43643951f, -90.58059692f, -91.79488373f, -91.8683548f, -90.79725647f, -89.46754456f, -89.03026581f, -87.65201569f, -87.0351944f, -86.88602448f, -87.51631927f, -89.20323181f, -91.95607758f, -94.29904175f, -95.63497162f, -95.79863739f, -94.77147675f, -93.70718384f, -93.07803345f, -92.13182831f, -90.4257431f, -89.27892303f, -89.21079254f, -90.39524078f, -92.53555298f, -95.23703003f, -96.17893982f, -93.93228912f, -92.60162354f, -91.40220642f, -91.05371094f, -92.28942108f, -94.27224731f, -95.10214233f, -93.62699127f, -92.14169312f, -91.52285767f, -91.61370087f, -91.79831696f, -91.60452271f, -91.47457123f, -91.85878754f, -93.00186157f, -94.56305695f, -95.55973816f, -95.15345764f, -93.93331909f, -91.6451416f};
        public static double GSMStep_Hz = 305.1757288094;
        public static int GSMCentralIndex = 819;
    }
}