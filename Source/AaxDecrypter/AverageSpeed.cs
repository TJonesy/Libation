﻿using Dinah.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AaxDecrypter;

public static class LinqStats
{
	public static (double mean, double stdDev) BasicStatisticsBy<T>(this IEnumerable<T> values, Func<T, double> selector)
	{
		var count = values.Count();
		var mean = values.Average(selector);

		return (mean, Math.Sqrt(values.Sum(s => Math.Pow(selector(s) - mean, 2)) / (count - 1)));
	}

	public static bool T_Test_2By<T>(this IEnumerable<T> values, Func<T, double> selector, IEnumerable<T> secondGroup, Significance confidence)
	{
		var n1 = values.Count();
		var n2 = secondGroup.Count();
		var n = n1 + n2;

		if (n1 < 3 || n2 < 3) return false;

		(var mean1, var stdDev1) = values.BasicStatisticsBy(selector);
		(var mean2, var stdDev2) = secondGroup.BasicStatisticsBy(selector);

		var pooledStdDev = Math.Sqrt((((n1 - 1) * (stdDev1 * stdDev1)) + ((n2 - 1) * (stdDev2 * stdDev2))) / (n1 + n2 - 2));

		var testStat = Math.Abs(mean1 - mean2) / (pooledStdDev * Math.Sqrt(1d / n1 + 1d / n2));
		var crit = T_Stat(Math.Min(n - 2, MAX_DEGREES_FREEDOM), confidence);

		return testStat > crit;
	}

	public static bool T_Test_1By<T>(this IEnumerable<T> values, Func<T, double> selector, double testMean, Significance confidence)
	{
		var n = values.Count();

		if (n < 2) return false;

		(var sampleMean, var sampleStdDev) = values.BasicStatisticsBy(selector);

		var testStat = Math.Abs(sampleMean - testMean) / (sampleStdDev / Math.Sqrt(n));
		var crit = T_Stat(Math.Min(n - 1, MAX_DEGREES_FREEDOM), confidence);

		return testStat > crit;
	}

	private static double T_Stat(int degreesFreedom, Significance confidence)
	{
		ArgumentValidator.EnsureBetweenInclusive(degreesFreedom, nameof(degreesFreedom), MIN_DEGREES_FREEDOM, MAX_DEGREES_FREEDOM);

		return T_TABLE[(int)confidence][degreesFreedom - MIN_DEGREES_FREEDOM];
	}

	static LinqStats()
	{
		T_TABLE = new double[][] { T_Table_01, T_Table_05, T_Table_10, T_Table_15, T_Table_20, T_Table_25 };
	}

	private const int MIN_DEGREES_FREEDOM = 1;
	private const int MAX_DEGREES_FREEDOM = 201;
	/// <summary>
	/// 2-tailed  t-Distribution critical values at 75%, 80%, 85%,
	/// 90%, 95%, and 99% confidence for 1 - 201 degrees of freedom.
	/// </summary>
	private readonly static double[][] T_TABLE;
	private readonly static double[] T_Table_25 = { 2.414213562, 1.603567451, 1.422625281, 1.344397556, 1.300949037, 1.273349309, 1.254278682, 1.240318261, 1.229659173, 1.221255395, 1.214460246, 1.208852542, 1.204146242, 1.200140298, 1.196689284, 1.193685414, 1.191047107, 1.188711483, 1.186629298, 1.184761434, 1.183076432, 1.181548697, 1.180157199, 1.178884497, 1.177716003, 1.176639425, 1.175644329, 1.174721803, 1.173864189, 1.173064871, 1.1723181, 1.17161886, 1.170962753, 1.17034591, 1.169764906, 1.169216709, 1.168698615, 1.168208212, 1.167743338, 1.167302049, 1.166882595, 1.166483396, 1.166103019, 1.165740162, 1.165393644, 1.165062385, 1.164745398, 1.164441782, 1.164150707, 1.163871412, 1.163603196, 1.163345413, 1.163097467, 1.162858803, 1.162628911, 1.162407316, 1.162193577, 1.161987283, 1.161788052, 1.161595527, 1.161409375, 1.161229286, 1.161054967, 1.160886145, 1.160722566, 1.160563987, 1.160410184, 1.160260944, 1.160116066, 1.159975363, 1.159838656, 1.159705777, 1.159576569, 1.15945088, 1.15932857, 1.159209503, 1.159093552, 1.158980598, 1.158870524, 1.158763222, 1.158658589, 1.158556526, 1.15845694, 1.158359742, 1.158264847, 1.158172173, 1.158081645, 1.157993188, 1.157906731, 1.157822209, 1.157739556, 1.157658712, 1.157579617, 1.157502216, 1.157426454, 1.157352281, 1.157279646, 1.157208502, 1.157138804, 1.157070509, 1.157003573, 1.156937958, 1.156873624, 1.156810534, 1.156748653, 1.156687945, 1.156628379, 1.156569922, 1.156512543, 1.156456213, 1.156400904, 1.156346587, 1.156293237, 1.156240827, 1.156189334, 1.156138733, 1.156089001, 1.156040117, 1.155992058, 1.155944804, 1.155898335, 1.155852631, 1.155807674, 1.155763446, 1.155719928, 1.155677105, 1.155634959, 1.155593475, 1.155552637, 1.15551243, 1.155472839, 1.155433851, 1.155395452, 1.155357629, 1.155320368, 1.155283658, 1.155247486, 1.155211841, 1.15517671, 1.155142084, 1.15510795, 1.1550743, 1.155041122, 1.155008406, 1.154976144, 1.154944326, 1.154912942, 1.154881984, 1.154851443, 1.154821311, 1.15479158, 1.154762241, 1.154733287, 1.154704711, 1.154676505, 1.154648662, 1.154621175, 1.154594037, 1.154567242, 1.154540783, 1.154514654, 1.154488849, 1.154463361, 1.154438185, 1.154413316, 1.154388747, 1.154364474, 1.15434049, 1.154316792, 1.154293373, 1.154270229, 1.154247355, 1.154224746, 1.154202398, 1.154180307, 1.154158467, 1.154136875, 1.154115526, 1.154094417, 1.154073543, 1.1540529, 1.154032485, 1.154012294, 1.153992323, 1.153972568, 1.153953027, 1.153933695, 1.15391457, 1.153895647, 1.153876925, 1.153858399, 1.153840066, 1.153821925, 1.15380397, 1.153786201, 1.153768613, 1.153751204, 1.153733972, 1.153716914, 1.153700026 };
	private readonly static double[] T_Table_20 = { 3.077683537, 1.885618083, 1.637744354, 1.533206274, 1.475884049, 1.439755747, 1.414923928, 1.39681531, 1.383028738, 1.372183641, 1.363430318, 1.356217334, 1.350171289, 1.345030374, 1.340605608, 1.336757167, 1.33337939, 1.330390944, 1.327728209, 1.325340707, 1.323187874, 1.321236742, 1.31946024, 1.317835934, 1.316345073, 1.314971864, 1.313702913, 1.312526782, 1.311433647, 1.310415025, 1.309463549, 1.308572793, 1.307737124, 1.306951587, 1.306211802, 1.305513886, 1.304854381, 1.304230204, 1.303638589, 1.303077053, 1.302543359, 1.302035487, 1.301551608, 1.30109006, 1.300649332, 1.300228048, 1.299824947, 1.299438879, 1.299068785, 1.298713694, 1.298372713, 1.298045016, 1.297729843, 1.297426488, 1.2971343, 1.296852673, 1.296581044, 1.29631889, 1.296065725, 1.295821094, 1.295584571, 1.295355762, 1.295134294, 1.29491982, 1.294712013, 1.294510568, 1.294315197, 1.294125629, 1.293941609, 1.293762898, 1.293589269, 1.293420507, 1.293256413, 1.293096793, 1.292941469, 1.292790268, 1.292643029, 1.292499597, 1.292359828, 1.292223583, 1.29209073, 1.291961144, 1.291834705, 1.291711301, 1.291590824, 1.291473171, 1.291358243, 1.291245948, 1.291136195, 1.291028899, 1.290923979, 1.290821356, 1.290720956, 1.290622708, 1.290526543, 1.290432395, 1.290340202, 1.290249904, 1.290161442, 1.290074761, 1.289989809, 1.289906533, 1.289824884, 1.289744816, 1.289666283, 1.289589241, 1.289513648, 1.289439464, 1.289366649, 1.289295166, 1.289224979, 1.289156054, 1.289088355, 1.289021851, 1.28895651, 1.288892302, 1.288829199, 1.288767171, 1.288706191, 1.288646234, 1.288587273, 1.288529284, 1.288472243, 1.288416127, 1.288360913, 1.288306581, 1.288253109, 1.288200477, 1.288148665, 1.288097654, 1.288047427, 1.287997964, 1.287949248, 1.287901264, 1.287853994, 1.287807422, 1.287761534, 1.287716314, 1.287671748, 1.287627821, 1.287584521, 1.287541833, 1.287499745, 1.287458245, 1.287417319, 1.287376957, 1.287337146, 1.287297876, 1.287259135, 1.287220914, 1.2871832, 1.287145985, 1.287109259, 1.287073012, 1.287037235, 1.287001918, 1.286967053, 1.286932631, 1.286898644, 1.286865084, 1.286831942, 1.286799212, 1.286766884, 1.286734952, 1.286703409, 1.286672248, 1.286641461, 1.286611042, 1.286580985, 1.286551283, 1.286521929, 1.286492918, 1.286464244, 1.286435901, 1.286407882, 1.286380184, 1.286352799, 1.286325724, 1.286298952, 1.286272479, 1.286246299, 1.286220408, 1.286194801, 1.286169474, 1.286144421, 1.286119638, 1.286095122, 1.286070867, 1.28604687, 1.286023127, 1.285999633, 1.285976384, 1.285953377, 1.285930609, 1.285908074, 1.285885771, 1.285863694, 1.285841842, 1.285820209, 1.285798794 };
	private readonly static double[] T_Table_15 = { 4.16529977, 2.281930588, 1.924319657, 1.778192164, 1.699362566, 1.650173154, 1.616591737, 1.59222144, 1.573735785, 1.559235933, 1.547559766, 1.537956495, 1.529919606, 1.523095061, 1.517227969, 1.51213017, 1.507659754, 1.503707672, 1.500188756, 1.497035518, 1.494193795, 1.491619612, 1.489276897, 1.487135783, 1.485171326, 1.483362535, 1.481691617, 1.48014339, 1.478704821, 1.477364662, 1.47611315, 1.474941772, 1.473843072, 1.47281049, 1.471838233, 1.470921166, 1.470054719, 1.469234815, 1.468457801, 1.467720399, 1.467019655, 1.466352901, 1.465717725, 1.465111933, 1.464533534, 1.463980712, 1.463451805, 1.462945295, 1.46245979, 1.461994009, 1.461546775, 1.461117, 1.460703683, 1.460305896, 1.45992278, 1.459553538, 1.45919743, 1.458853767, 1.458521908, 1.458201256, 1.457891251, 1.457591373, 1.457301133, 1.457020074, 1.456747768, 1.45648381, 1.456227824, 1.455979454, 1.455738365, 1.455504241, 1.455276784, 1.455055715, 1.454840767, 1.45463169, 1.454428246, 1.454230212, 1.454037373, 1.453849529, 1.453666487, 1.453488066, 1.453314093, 1.453144404, 1.452978842, 1.452817259, 1.452659513, 1.452505469, 1.452354998, 1.452207977, 1.452064289, 1.451923821, 1.451786468, 1.451652126, 1.451520697, 1.451392088, 1.451266209, 1.451142973, 1.451022299, 1.450904108, 1.450788323, 1.450674871, 1.450563684, 1.450454694, 1.450347836, 1.450243048, 1.450140271, 1.450039448, 1.449940523, 1.449843444, 1.449748158, 1.449654617, 1.449562773, 1.449472581, 1.449383997, 1.449296977, 1.449211481, 1.449127468, 1.449044902, 1.448963744, 1.448883959, 1.448805513, 1.448728372, 1.448652503, 1.448577876, 1.44850446, 1.448432226, 1.448361146, 1.448291192, 1.448222337, 1.448154557, 1.448087826, 1.44802212, 1.447957415, 1.447893688, 1.447830919, 1.447769085, 1.447708165, 1.44764814, 1.44758899, 1.447530695, 1.447473238, 1.447416601, 1.447360765, 1.447305715, 1.447251433, 1.447197905, 1.447145113, 1.447093044, 1.447041682, 1.446991013, 1.446941023, 1.446891698, 1.446843026, 1.446794994, 1.446747588, 1.446700797, 1.446654609, 1.446609012, 1.446563996, 1.446519548, 1.446475659, 1.446432318, 1.446389514, 1.446347238, 1.44630548, 1.44626423, 1.44622348, 1.44618322, 1.446143442, 1.446104137, 1.446065296, 1.446026911, 1.445988975, 1.44595148, 1.445914417, 1.44587778, 1.445841561, 1.445805753, 1.445770349, 1.445735343, 1.445700727, 1.445666495, 1.445632641, 1.445599159, 1.445566042, 1.445533284, 1.445500881, 1.445468825, 1.445437112, 1.445405736, 1.445374691, 1.445343973, 1.445313576, 1.445283495, 1.445253726, 1.445224264, 1.445195103, 1.445166239, 1.445137668, 1.445109385, 1.445081387 };
	private readonly static double[] T_Table_10 = { 6.313751515, 2.91998558, 2.353363435, 2.131846786, 2.015048373, 1.943180281, 1.894578605, 1.859548038, 1.833112933, 1.812461123, 1.795884819, 1.782287556, 1.770933396, 1.761310136, 1.753050356, 1.745883676, 1.739606726, 1.734063607, 1.729132812, 1.724718243, 1.720742903, 1.717144374, 1.713871528, 1.71088208, 1.708140761, 1.70561792, 1.703288446, 1.701130934, 1.699127027, 1.697260887, 1.695518783, 1.693888748, 1.692360309, 1.690924255, 1.689572458, 1.688297714, 1.68709362, 1.68595446, 1.684875122, 1.683851013, 1.682878002, 1.681952357, 1.681070703, 1.680229977, 1.679427393, 1.678660414, 1.677926722, 1.677224196, 1.676550893, 1.675905025, 1.67528495, 1.674689154, 1.674116237, 1.673564906, 1.673033965, 1.672522303, 1.672028888, 1.671552762, 1.671093032, 1.670648865, 1.670219484, 1.669804163, 1.669402222, 1.669013025, 1.668635976, 1.668270514, 1.667916114, 1.667572281, 1.667238549, 1.666914479, 1.666599658, 1.666293696, 1.665996224, 1.665706893, 1.665425373, 1.665151353, 1.664884537, 1.664624645, 1.664371409, 1.664124579, 1.663883913, 1.663649184, 1.663420175, 1.663196679, 1.6629785, 1.662765449, 1.662557349, 1.662354029, 1.662155326, 1.661961084, 1.661771155, 1.661585397, 1.661403674, 1.661225855, 1.661051817, 1.66088144, 1.66071461, 1.660551217, 1.660391156, 1.660234326, 1.66008063, 1.659929976, 1.659782273, 1.659637437, 1.659495383, 1.659356034, 1.659219312, 1.659085144, 1.658953458, 1.658824187, 1.658697265, 1.658572629, 1.658450216, 1.658329969, 1.65821183, 1.658095744, 1.657981659, 1.657869522, 1.657759285, 1.657650899, 1.657544319, 1.657439499, 1.657336397, 1.65723497, 1.657135178, 1.657036982, 1.656940344, 1.656845226, 1.656751594, 1.656659413, 1.656568649, 1.65647927, 1.656391244, 1.656304542, 1.656219133, 1.656134988, 1.65605208, 1.655970382, 1.655889868, 1.655810511, 1.655732287, 1.655655173, 1.655579143, 1.655504177, 1.655430251, 1.655357345, 1.655285437, 1.655214506, 1.655144534, 1.6550755, 1.655007387, 1.654940175, 1.654873847, 1.654808385, 1.654743774, 1.654679996, 1.654617035, 1.654554875, 1.654493503, 1.654432901, 1.654373057, 1.654313957, 1.654255585, 1.654197929, 1.654140976, 1.654084713, 1.654029128, 1.653974208, 1.653919942, 1.653866317, 1.653813324, 1.653760949, 1.653709184, 1.653658017, 1.653607437, 1.653557435, 1.653508002, 1.653459126, 1.6534108, 1.653363013, 1.653315758, 1.653269024, 1.653222803, 1.653177088, 1.653131869, 1.653087138, 1.653042889, 1.652999113, 1.652955802, 1.652912949, 1.652870547, 1.652828589, 1.652787068, 1.652745977, 1.65270531, 1.652665059, 1.652625219, 1.652585784, 1.652546746, 1.652508101 };
	private readonly static double[] T_Table_05 = { 12.70620474, 4.30265273, 3.182446305, 2.776445105, 2.570581836, 2.446911851, 2.364624252, 2.306004135, 2.262157163, 2.228138852, 2.20098516, 2.17881283, 2.160368656, 2.144786688, 2.131449546, 2.119905299, 2.109815578, 2.10092204, 2.093024054, 2.085963447, 2.079613845, 2.073873068, 2.06865761, 2.063898562, 2.059538553, 2.055529439, 2.051830516, 2.048407142, 2.045229642, 2.042272456, 2.039513446, 2.036933343, 2.034515297, 2.032244509, 2.030107928, 2.028094001, 2.026192463, 2.024394164, 2.02269092, 2.02107539, 2.01954097, 2.018081703, 2.016692199, 2.015367574, 2.014103389, 2.012895599, 2.011740514, 2.010634758, 2.009575237, 2.008559112, 2.00758377, 2.006646805, 2.005745995, 2.004879288, 2.004044783, 2.003240719, 2.002465459, 2.001717484, 2.000995378, 2.000297822, 1.999623585, 1.998971517, 1.998340543, 1.997729654, 1.997137908, 1.996564419, 1.996008354, 1.995468931, 1.994945415, 1.994437112, 1.993943368, 1.993463567, 1.992997126, 1.992543495, 1.992102154, 1.99167261, 1.991254395, 1.990847069, 1.99045021, 1.990063421, 1.989686323, 1.989318557, 1.98895978, 1.988609667, 1.988267907, 1.987934206, 1.987608282, 1.987289865, 1.9869787, 1.986674541, 1.986377154, 1.986086317, 1.985801814, 1.985523442, 1.985251004, 1.984984312, 1.984723186, 1.984467455, 1.984216952, 1.983971519, 1.983731003, 1.983495259, 1.983264145, 1.983037526, 1.982815274, 1.982597262, 1.98238337, 1.982173483, 1.98196749, 1.981765282, 1.981566757, 1.981371815, 1.981180359, 1.980992298, 1.980807541, 1.980626002, 1.980447599, 1.980272249, 1.980099876, 1.979930405, 1.979763763, 1.979599878, 1.979438685, 1.979280117, 1.979124109, 1.978970602, 1.978819535, 1.97867085, 1.978524491, 1.978380405, 1.978238539, 1.978098842, 1.977961264, 1.977825758, 1.977692277, 1.977560777, 1.977431212, 1.977303542, 1.977177724, 1.97705372, 1.976931489, 1.976810994, 1.976692198, 1.976575066, 1.976459563, 1.976345655, 1.976233309, 1.976122494, 1.976013178, 1.975905331, 1.975798924, 1.975693928, 1.975590315, 1.975488058, 1.975387131, 1.975287508, 1.975189163, 1.975092073, 1.974996213, 1.97490156, 1.974808092, 1.974715786, 1.974624621, 1.974534576, 1.97444563, 1.974357764, 1.974270957, 1.974185191, 1.974100447, 1.974016708, 1.973933954, 1.973852169, 1.973771337, 1.97369144, 1.973612462, 1.973534388, 1.973457202, 1.973380889, 1.973305434, 1.973230823, 1.973157042, 1.973084077, 1.973011915, 1.972940542, 1.972869946, 1.972800114, 1.972731033, 1.972662692, 1.972595079, 1.972528182, 1.97246199, 1.972396491, 1.972331676, 1.972267533, 1.972204051, 1.972141222, 1.972079034, 1.972017478, 1.971956544, 1.971896224 };
	private readonly static double[] T_Table_01 = { 63.65674116, 9.924843201, 5.84090931, 4.604094871, 4.032142984, 3.707428021, 3.499483297, 3.355387331, 3.249835542, 3.169272673, 3.105806516, 3.054539589, 3.012275839, 2.976842734, 2.946712883, 2.920781622, 2.89823052, 2.878440473, 2.860934606, 2.84533971, 2.831359558, 2.818756061, 2.807335684, 2.796939505, 2.787435814, 2.778714533, 2.770682957, 2.763262455, 2.756385904, 2.749995654, 2.744041919, 2.738481482, 2.733276642, 2.728394367, 2.723805589, 2.71948463, 2.715408722, 2.711557602, 2.707913184, 2.704459267, 2.701181304, 2.698066186, 2.695102079, 2.692278266, 2.689585019, 2.687013492, 2.684555618, 2.682204027, 2.679951974, 2.677793271, 2.675722234, 2.673733631, 2.671822636, 2.669984796, 2.668215988, 2.666512398, 2.664870482, 2.663286954, 2.661758752, 2.660283029, 2.658857127, 2.657478565, 2.656145025, 2.654854337, 2.653604469, 2.652393515, 2.651219685, 2.650081299, 2.648976774, 2.647904624, 2.646863444, 2.645851913, 2.644868782, 2.643912872, 2.642983067, 2.642078313, 2.641197611, 2.640340015, 2.639504627, 2.638690596, 2.637897113, 2.63712341, 2.636368757, 2.635632458, 2.634913852, 2.634212309, 2.633527229, 2.632858038, 2.632204191, 2.631565166, 2.630940463, 2.630329608, 2.629732145, 2.629147638, 2.628575671, 2.628015844, 2.627467774, 2.626931096, 2.626405457, 2.625890521, 2.625385965, 2.624891476, 2.624406758, 2.623931523, 2.623465496, 2.623008411, 2.622560015, 2.622120061, 2.621688313, 2.621264543, 2.620848534, 2.620440073, 2.620038957, 2.619644989, 2.619257981, 2.618877749, 2.618504116, 2.618136914, 2.617775976, 2.617421145, 2.617072266, 2.616729191, 2.616391776, 2.616059883, 2.615733377, 2.615412127, 2.615096008, 2.614784899, 2.61447868, 2.614177238, 2.613880461, 2.613588242, 2.613300477, 2.613017065, 2.612737908, 2.61246291, 2.61219198, 2.611925028, 2.611661966, 2.611402711, 2.611147181, 2.610895295, 2.610646976, 2.61040215, 2.610160742, 2.609922682, 2.609687901, 2.609456331, 2.609227907, 2.609002566, 2.608780245, 2.608560883, 2.608344423, 2.608130807, 2.60791998, 2.607711886, 2.607506474, 2.607303692, 2.607103489, 2.606905817, 2.606710628, 2.606517876, 2.606327515, 2.606139501, 2.605953791, 2.605770342, 2.605589114, 2.605410067, 2.605233162, 2.605058359, 2.604885623, 2.604714916, 2.604546204, 2.60437945, 2.604214622, 2.604051686, 2.60389061, 2.603731363, 2.603573912, 2.603418229, 2.603264282, 2.603112045, 2.602961487, 2.602812582, 2.602665303, 2.602519622, 2.602375515, 2.602232955, 2.602091918, 2.60195238, 2.601814317, 2.601677705, 2.601542523, 2.601408747, 2.601276355, 2.601145327, 2.601015642, 2.600887278, 2.600760216, 2.600634436 };
}

public enum Significance
{
	P01,
	P05,
	P10,
	P15,
	P20,
	P25
}

public class AverageSpeed
{
	/// <summary>Average speed in units per second</summary>
	public double Average { get; private set; }
	public TimeSpan SlowWindow { get; }
	public TimeSpan FastWindow { get; }
	public Significance SlowSignificance { get; }
	public Significance FastSignificance { get; }

	private DateTime start;
	private TimeSpan lastTime;
	private double lastPosition = double.NaN;

	private readonly record struct Point(TimeSpan Time, double Velocity);
	private readonly LinkedList<Point> speeds = new();
	private const int MAX_SPEEDS = 200;

	public AverageSpeed() : this(TimeSpan.FromSeconds(15), Significance.P10, TimeSpan.FromSeconds(3), Significance.P01) { }

	/// <param name="slowWindow">Total moving average time window</param>
	/// <param name="slowSignificance">T-test signifance level at which the newest speed will be considered different from the slow window's mean speed.</param>
	/// <param name="fastWindow">A shorter moving window of the most resent speeds. The average speed in <paramref name="fastWindow"/> is compared to the average speed in the rest of <paramref name="slowWindow"/> to quickly detect large changes in speed.</param>
	/// <param name="fastSignificance">T-test significance level at which the mean speed in <paramref name="fastWindow"/> will be considered different from the mean speed of the remainder of <paramref name="slowWindow"/>.</param>
	public AverageSpeed(TimeSpan slowWindow, Significance slowSignificance, TimeSpan fastWindow, Significance fastSignificance)
	{
		SlowWindow = ArgumentValidator.EnsureGreaterThan(slowWindow, nameof(slowWindow), fastWindow);
		FastWindow = ArgumentValidator.EnsureGreaterThan(fastWindow, nameof(fastWindow), TimeSpan.Zero);
		SlowSignificance = slowSignificance;
		FastSignificance = fastSignificance;
	}

	/// <summary>Add a new position to the moving average</summary>
	public void AddPosition(double position)
	{
		var now = DateTime.Now;
		if (start == default)
			start = now;

		var time = now - start;

		while (speeds.Count > MAX_SPEEDS || (speeds.Count > 2 && time - speeds.First.Value.Time > SlowWindow))
			speeds.RemoveFirst();

		if (!double.IsNaN(lastPosition))
		{
			var newSpeed = (position - lastPosition) / (time - lastTime).TotalSeconds;
			speeds.AddLast(new Point(time, newSpeed));
		}

		lastTime = time;
		lastPosition = position;

		Average = ComputeNextAverage();
	}

	private double ComputeNextAverage()
	{
		if (speeds.Count == 0)
			return 0;
		else if (speeds.Count == 1)
			return speeds.Last.Value.Velocity;
		else
		{
			var n_newest = speeds.Count(s => s.Time > lastTime.Subtract(FastWindow));

			var n_oldest = speeds.Count - n_newest;

			if (speeds.Take(n_oldest).T_Test_2By(s => s.Velocity, speeds.TakeLast(n_newest), FastSignificance))
			{
				//Speeds in FastWindow are significantly different from reset of speeds in SlowWindow.
				//Discard older speeds and keep only speeds in FastWindow
				for (; n_oldest > 0; n_oldest--)
					speeds.RemoveFirst();

				return speeds.Average(s => s.Velocity);
			}
			else
				return
					speeds.T_Test_1By(s => s.Velocity, Average, SlowSignificance)
					? speeds.Average(s => s.Velocity)
					: Average;
		}
	}
}
