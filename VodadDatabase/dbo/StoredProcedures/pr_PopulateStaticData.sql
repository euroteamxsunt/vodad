CREATE PROCEDURE [dbo].[pr_PopulateStaticData]
AS
BEGIN
	DELETE FROM [dbo].[Merchants]

	INSERT INTO [dbo].[Merchants]
	SELECT 'PayPal'

	DELETE FROM [dbo].[Roles]

	INSERT INTO [dbo].[Roles]
	SELECT 'Administrator'
	UNION ALL
	SELECT 'Helper'
	UNION ALL
	SELECT 'Performer'
	UNION ALL
	SELECT 'Advertiser'
	UNION ALL
	SELECT 'Banned'

	DELETE FROM [dbo].[TicketThemes]

	INSERT INTO [dbo].[TicketThemes]
	SELECT 'Wallet'
	UNION ALL
	SELECT 'Orders'
	UNION ALL
	SELECT 'Platform'
	UNION ALL
	SELECT 'Messages'
	UNION ALL
	SELECT 'Other'

	DELETE FROM [dbo].[Regions]

	INSERT INTO [dbo].[Regions]
	SELECT 'Any'
	UNION ALL
	SELECT 'Eastern Africa'
	UNION ALL
	SELECT 'Middle Africa'
	UNION ALL
	SELECT 'Northern Africa'
	UNION ALL
	SELECT 'Southern Africa'
	UNION ALL
	SELECT 'Western Africa'
	UNION ALL
	SELECT 'Latin America'
	UNION ALL
	SELECT 'Northern America'
	UNION ALL
	SELECT 'Antarctica'
	UNION ALL
	SELECT 'Central Asia'
	UNION ALL
	SELECT 'Eastern Asia'
	UNION ALL
	SELECT 'Southern Asia'
	UNION ALL
	SELECT 'South-Eastern Asia'
	UNION ALL
	SELECT 'Western Asia'
	UNION ALL
	SELECT 'Eastern Europe'
	UNION ALL
	SELECT 'Northern Europe'
	UNION ALL
	SELECT 'Southern Europe'
	UNION ALL
	SELECT 'Western Europe'
	UNION ALL
	SELECT 'Australia and New Zealand'
	UNION ALL
	SELECT 'Melanesia'
	UNION ALL
	SELECT 'Micronesia'
	UNION ALL
	SELECT 'Polynesia'

	DELETE FROM [dbo].[Timezone]

	INSERT INTO [dbo].[Timezone]
	SELECT '-12'
	UNION ALL
	SELECT '-11'
	UNION ALL
	SELECT '-10'
	UNION ALL
	SELECT '-9:30'
	UNION ALL
	SELECT '-9'
	UNION ALL
	SELECT '-8:30'
	UNION ALL
	SELECT '-7'
	UNION ALL
	SELECT '-6'
	UNION ALL
	SELECT '-5'
	UNION ALL
	SELECT '-4:30'
	UNION ALL
	SELECT '-4'
	UNION ALL
	SELECT '-3:30'
	UNION ALL
	SELECT '-3'
	UNION ALL
	SELECT '-2:30'
	UNION ALL
	SELECT '-2'
	UNION ALL
	SELECT '-1'
	UNION ALL
	SELECT '-0:44'
	UNION ALL
	SELECT '-0:25'
	UNION ALL
	SELECT 'UTC 0:00'
	UNION ALL
	SELECT '+0:20'
	UNION ALL
	SELECT '+0:30'
	UNION ALL
	SELECT '+1'
	UNION ALL
	SELECT '+2'
	UNION ALL
	SELECT '+3'
	UNION ALL
	SELECT '+3:30'
	UNION ALL
	SELECT '+4'
	UNION ALL
	SELECT '+4:30'
	UNION ALL
	SELECT '+4:51'
	UNION ALL
	SELECT '+5'
	UNION ALL
	SELECT '+5:30'
	UNION ALL
	SELECT '+5:40'
	UNION ALL
	SELECT '+5:45'
	UNION ALL
	SELECT '6'
	UNION ALL
	SELECT '+6:30'
	UNION ALL
	SELECT '+7'
	UNION ALL
	SELECT '+7:30'
	UNION ALL
	SELECT '+8'
	UNION ALL
	SELECT '+8:45'
	UNION ALL
	SELECT '+9'
	UNION ALL
	SELECT '+9:30'
	UNION ALL
	SELECT '+10'
	UNION ALL
	SELECT '+10:30'
	UNION ALL
	SELECT '+11'
	UNION ALL
	SELECT '+11:30'
	UNION ALL
	SELECT '+12'
	UNION ALL
	SELECT '+12:45'
	UNION ALL
	SELECT '+13'
	UNION ALL
	SELECT '+13:45'
	UNION ALL
	SELECT '+14'

	DELETE FROM [dbo].[Themes]

	INSERT INTO [dbo].[Themes]
	SELECT 'Popular'
	UNION ALL
	SELECT 'News'
	UNION ALL
	SELECT 'Entertainment'
	UNION ALL
	SELECT 'Sport'
	UNION ALL
	SELECT 'Animals & Wildlife'
	UNION ALL
	SELECT 'Music'
	UNION ALL
	SELECT 'Technology'
	UNION ALL
	SELECT 'Gaming'
	UNION ALL
	SELECT 'Education'

	DELETE FROM [dbo].[Geolocation]

	INSERT INTO [dbo].[Geolocation] 
	(
		[ISO2],
		[CountryName],
		[LongCountryName],
		[ISO3],
		[NumCode],
		[UNMemberState],
		[CallingCode],
		[CCTLD]
	)
	VALUES
	('AF','Afghanistan','Islamic Republic of Afghanistan','AFG','004','yes','93','.af'),
	('AX','Aland Islands','&Aring;land Islands','ALA','248','no','358','.ax'),
	('AL','Albania','Republic of Albania','ALB','008','yes','355','.al'),
	('DZ','Algeria','People''s Democratic Republic of Algeria','DZA','012','yes','213','.dz'),
	('AS','American Samoa','American Samoa','ASM','016','no','1+684','.as'),
	('AD','Andorra','Principality of Andorra','AND','020','yes','376','.ad'),
	('AO','Angola','Republic of Angola','AGO','024','yes','244','.ao'),
	('AI','Anguilla','Anguilla','AIA','660','no','1+264','.ai'),
	('AQ','Antarctica','Antarctica','ATA','010','no','672','.aq'),
	('AG','Antigua and Barbuda','Antigua and Barbuda','ATG','028','yes','1+268','.ag'),
	('AR','Argentina','Argentine Republic','ARG','032','yes','54','.ar'),
	('AM','Armenia','Republic of Armenia','ARM','051','yes','374','.am'),
	('AW','Aruba','Aruba','ABW','533','no','297','.aw'),
	('AU','Australia','Commonwealth of Australia','AUS','036','yes','61','.au'),
	('AT','Austria','Republic of Austria','AUT','040','yes','43','.at'),
	('AZ','Azerbaijan','Republic of Azerbaijan','AZE','031','yes','994','.az'),
	('BS','Bahamas','Commonwealth of The Bahamas','BHS','044','yes','1+242','.bs'),
	('BH','Bahrain','Kingdom of Bahrain','BHR','048','yes','973','.bh'),
	('BD','Bangladesh','People''s Republic of Bangladesh','BGD','050','yes','880','.bd'),
	('BB','Barbados','Barbados','BRB','052','yes','1+246','.bb'),
	('BY','Belarus','Republic of Belarus','BLR','112','yes','375','.by'),
	('BE','Belgium','Kingdom of Belgium','BEL','056','yes','32','.be'),
	('BZ','Belize','Belize','BLZ','084','yes','501','.bz'),
	('BJ','Benin','Republic of Benin','BEN','204','yes','229','.bj'),
	('BM','Bermuda','Bermuda Islands','BMU','060','no','1+441','.bm'),
	('BT','Bhutan','Kingdom of Bhutan','BTN','064','yes','975','.bt'),
	('BO','Bolivia','Plurinational State of Bolivia','BOL','068','yes','591','.bo'),
	('BQ','Bonaire, Sint Eustatius and Saba','Bonaire, Sint Eustatius and Saba','BES','535','no','599','.bq'),
	('BA','Bosnia and Herzegovina','Bosnia and Herzegovina','BIH','070','yes','387','.ba'),
	('BW','Botswana','Republic of Botswana','BWA','072','yes','267','.bw'),
	('BV','Bouvet Island','Bouvet Island','BVT','074','no','NONE','.bv'),
	('BR','Brazil','Federative Republic of Brazil','BRA','076','yes','55','.br'),
	('IO','British Indian Ocean Territory','British Indian Ocean Territory','IOT','086','no','246','.io'),
	('BN','Brunei','Brunei Darussalam','BRN','096','yes','673','.bn'),
	('BG','Bulgaria','Republic of Bulgaria','BGR','100','yes','359','.bg'),
	('BF','Burkina Faso','Burkina Faso','BFA','854','yes','226','.bf'),
	('BI','Burundi','Republic of Burundi','BDI','108','yes','257','.bi'),
	('KH','Cambodia','Kingdom of Cambodia','KHM','116','yes','855','.kh'),
	('CM','Cameroon','Republic of Cameroon','CMR','120','yes','237','.cm'),
	('CA','Canada','Canada','CAN','124','yes','1','.ca'),
	('CV','Cape Verde','Republic of Cape Verde','CPV','132','yes','238','.cv'),
	('KY','Cayman Islands','The Cayman Islands','CYM','136','no','1+345','.ky'),
	('CF','Central African Republic','Central African Republic','CAF','140','yes','236','.cf'),
	('TD','Chad','Republic of Chad','TCD','148','yes','235','.td'),
	('CL','Chile','Republic of Chile','CHL','152','yes','56','.cl'),
	('CN','China','People''s Republic of China','CHN','156','yes','86','.cn'),
	('CX','Christmas Island','Christmas Island','CXR','162','no','61','.cx'),
	('CC','Cocos (Keeling) Islands','Cocos (Keeling) Islands','CCK','166','no','61','.cc'),
	('CO','Colombia','Republic of Colombia','COL','170','yes','57','.co'),
	('KM','Comoros','Union of the Comoros','COM','174','yes','269','.km'),
	('CG','Congo','Republic of the Congo','COG','178','yes','242','.cg'),
	('CK','Cook Islands','Cook Islands','COK','184','some','682','.ck'),
	('CR','Costa Rica','Republic of Costa Rica','CRI','188','yes','506','.cr'),
	('CI','Cote d''ivoire (Ivory Coast)','Republic of C&ocirc;te D''Ivoire (Ivory Coast)','CIV','384','yes','225','.ci'),
	('HR','Croatia','Republic of Croatia','HRV','191','yes','385','.hr'),
	('CU','Cuba','Republic of Cuba','CUB','192','yes','53','.cu'),
	('CW','Curacao','Cura&ccedil;ao','CUW','531','no','599','.cw'),
	('CY','Cyprus','Republic of Cyprus','CYP','196','yes','357','.cy'),
	('CZ','Czech Republic','Czech Republic','CZE','203','yes','420','.cz'),
	('CD','Democratic Republic of the Congo','Democratic Republic of the Congo','COD','180','yes','243','.cd'),
	('DK','Denmark','Kingdom of Denmark','DNK','208','yes','45','.dk'),
	('DJ','Djibouti','Republic of Djibouti','DJI','262','yes','253','.dj'),
	('DM','Dominica','Commonwealth of Dominica','DMA','212','yes','1+767','.dm'),
	('DO','Dominican Republic','Dominican Republic','DOM','214','yes','1+809, 8','.do'),
	('EC','Ecuador','Republic of Ecuador','ECU','218','yes','593','.ec'),
	('EG','Egypt','Arab Republic of Egypt','EGY','818','yes','20','.eg'),
	('SV','El Salvador','Republic of El Salvador','SLV','222','yes','503','.sv'),
	('GQ','Equatorial Guinea','Republic of Equatorial Guinea','GNQ','226','yes','240','.gq'),
	('ER','Eritrea','State of Eritrea','ERI','232','yes','291','.er'),
	('EE','Estonia','Republic of Estonia','EST','233','yes','372','.ee'),
	('ET','Ethiopia','Federal Democratic Republic of Ethiopia','ETH','231','yes','251','.et'),
	('FK','Falkland Islands (Malvinas)','The Falkland Islands (Malvinas)','FLK','238','no','500','.fk'),
	('FO','Faroe Islands','The Faroe Islands','FRO','234','no','298','.fo'),
	('FJ','Fiji','Republic of Fiji','FJI','242','yes','679','.fj'),
	('FI','Finland','Republic of Finland','FIN','246','yes','358','.fi'),
	('FR','France','French Republic','FRA','250','yes','33','.fr'),
	('GF','French Guiana','French Guiana','GUF','254','no','594','.gf'),
	('PF','French Polynesia','French Polynesia','PYF','258','no','689','.pf'),
	('TF','French Southern Territories','French Southern Territories','ATF','260','no','NULL','.tf'),
	('GA','Gabon','Gabonese Republic','GAB','266','yes','241','.ga'),
	('GM','Gambia','Republic of The Gambia','GMB','270','yes','220','.gm'),
	('GE','Georgia','Georgia','GEO','268','yes','995','.ge'),
	('DE','Germany','Federal Republic of Germany','DEU','276','yes','49','.de'),
	('GH','Ghana','Republic of Ghana','GHA','288','yes','233','.gh'),
	('GI','Gibraltar','Gibraltar','GIB','292','no','350','.gi'),
	('GR','Greece','Hellenic Republic','GRC','300','yes','30','.gr'),
	('GL','Greenland','Greenland','GRL','304','no','299','.gl'),
	('GD','Grenada','Grenada','GRD','308','yes','1+473','.gd'),
	('GP','Guadaloupe','Guadeloupe','GLP','312','no','590','.gp'),
	('GU','Guam','Guam','GUM','316','no','1+671','.gu'),
	('GT','Guatemala','Republic of Guatemala','GTM','320','yes','502','.gt'),
	('GG','Guernsey','Guernsey','GGY','831','no','44','.gg'),
	('GN','Guinea','Republic of Guinea','GIN','324','yes','224','.gn'),
	('GW','Guinea-Bissau','Republic of Guinea-Bissau','GNB','624','yes','245','.gw'),
	('GY','Guyana','Co-operative Republic of Guyana','GUY','328','yes','592','.gy'),
	('HT','Haiti','Republic of Haiti','HTI','332','yes','509','.ht'),
	('HM','Heard Island and McDonald Islands','Heard Island and McDonald Islands','HMD','334','no','NONE','.hm'),
	('HN','Honduras','Republic of Honduras','HND','340','yes','504','.hn'),
	('HK','Hong Kong','Hong Kong','HKG','344','no','852','.hk'),
	('HU','Hungary','Hungary','HUN','348','yes','36','.hu'),
	('IS','Iceland','Republic of Iceland','ISL','352','yes','354','.is'),
	('IN','India','Republic of India','IND','356','yes','91','.in'),
	('ID','Indonesia','Republic of Indonesia','IDN','360','yes','62','.id'),
	('IR','Iran','Islamic Republic of Iran','IRN','364','yes','98','.ir'),
	('IQ','Iraq','Republic of Iraq','IRQ','368','yes','964','.iq'),
	('IE','Ireland','Ireland','IRL','372','yes','353','.ie'),
	('IM','Isle of Man','Isle of Man','IMN','833','no','44','.im'),
	('IL','Israel','State of Israel','ISR','376','yes','972','.il'),
	('IT','Italy','Italian Republic','ITA','380','yes','39','.jm'),
	('JM','Jamaica','Jamaica','JAM','388','yes','1+876','.jm'),
	('JP','Japan','Japan','JPN','392','yes','81','.jp'),
	('JE','Jersey','The Bailiwick of Jersey','JEY','832','no','44','.je'),
	('JO','Jordan','Hashemite Kingdom of Jordan','JOR','400','yes','962','.jo'),
	('KZ','Kazakhstan','Republic of Kazakhstan','KAZ','398','yes','7','.kz'),
	('KE','Kenya','Republic of Kenya','KEN','404','yes','254','.ke'),
	('KI','Kiribati','Republic of Kiribati','KIR','296','yes','686','.ki'),
	('XK','Kosovo','Republic of Kosovo','---','---','some','381',''),
	('KW','Kuwait','State of Kuwait','KWT','414','yes','965','.kw'),
	('KG','Kyrgyzstan','Kyrgyz Republic','KGZ','417','yes','996','.kg'),
	('LA','Laos','Lao People''s Democratic Republic','LAO','418','yes','856','.la'),
	('LV','Latvia','Republic of Latvia','LVA','428','yes','371','.lv'),
	('LB','Lebanon','Republic of Lebanon','LBN','422','yes','961','.lb'),
	('LS','Lesotho','Kingdom of Lesotho','LSO','426','yes','266','.ls'),
	('LR','Liberia','Republic of Liberia','LBR','430','yes','231','.lr'),
	('LY','Libya','Libya','LBY','434','yes','218','.ly'),
	('LI','Liechtenstein','Principality of Liechtenstein','LIE','438','yes','423','.li'),
	('LT','Lithuania','Republic of Lithuania','LTU','440','yes','370','.lt'),
	('LU','Luxembourg','Grand Duchy of Luxembourg','LUX','442','yes','352','.lu'),
	('MO','Macao','The Macao Special Administrative Region','MAC','446','no','853','.mo'),
	('MK','Macedonia','The Former Yugoslav Republic of Macedonia','MKD','807','yes','389','.mk'),
	('MG','Madagascar','Republic of Madagascar','MDG','450','yes','261','.mg'),
	('MW','Malawi','Republic of Malawi','MWI','454','yes','265','.mw'),
	('MY','Malaysia','Malaysia','MYS','458','yes','60','.my'),
	('MV','Maldives','Republic of Maldives','MDV','462','yes','960','.mv'),
	('ML','Mali','Republic of Mali','MLI','466','yes','223','.ml'),
	('MT','Malta','Republic of Malta','MLT','470','yes','356','.mt'),
	('MH','Marshall Islands','Republic of the Marshall Islands','MHL','584','yes','692','.mh'),
	('MQ','Martinique','Martinique','MTQ','474','no','596','.mq'),
	('MR','Mauritania','Islamic Republic of Mauritania','MRT','478','yes','222','.mr'),
	('MU','Mauritius','Republic of Mauritius','MUS','480','yes','230','.mu'),
	('YT','Mayotte','Mayotte','MYT','175','no','262','.yt'),
	('MX','Mexico','United Mexican States','MEX','484','yes','52','.mx'),
	('FM','Micronesia','Federated States of Micronesia','FSM','583','yes','691','.fm'),
	('MD','Moldava','Republic of Moldova','MDA','498','yes','373','.md'),
	('MC','Monaco','Principality of Monaco','MCO','492','yes','377','.mc'),
	('MN','Mongolia','Mongolia','MNG','496','yes','976','.mn'),
	('ME','Montenegro','Montenegro','MNE','499','yes','382','.me'),
	('MS','Montserrat','Montserrat','MSR','500','no','1+664','.ms'),
	('MA','Morocco','Kingdom of Morocco','MAR','504','yes','212','.ma'),
	('MZ','Mozambique','Republic of Mozambique','MOZ','508','yes','258','.mz'),
	('MM','Myanmar (Burma)','Republic of the Union of Myanmar','MMR','104','yes','95','.mm'),
	('NA','Namibia','Republic of Namibia','NAM','516','yes','264','.na'),
	('NR','Nauru','Republic of Nauru','NRU','520','yes','674','.nr'),
	('NP','Nepal','Federal Democratic Republic of Nepal','NPL','524','yes','977','.np'),
	('NL','Netherlands','Kingdom of the Netherlands','NLD','528','yes','31','.nl'),
	('NC','New Caledonia','New Caledonia','NCL','540','no','687','.nc'),
	('NZ','New Zealand','New Zealand','NZL','554','yes','64','.nz'),
	('NI','Nicaragua','Republic of Nicaragua','NIC','558','yes','505','.ni'),
	('NE','Niger','Republic of Niger','NER','562','yes','227','.ne'),
	('NG','Nigeria','Federal Republic of Nigeria','NGA','566','yes','234','.ng'),
	('NU','Niue','Niue','NIU','570','some','683','.nu'),
	('NF','Norfolk Island','Norfolk Island','NFK','574','no','672','.nf'),
	('KP','North Korea','Democratic People''s Republic of Korea','PRK','408','yes','850','.kp'),
	('MP','Northern Mariana Islands','Northern Mariana Islands','MNP','580','no','1+670','.mp'),
	('NO','Norway','Kingdom of Norway','NOR','578','yes','47','.no'),
	('OM','Oman','Sultanate of Oman','OMN','512','yes','968','.om'),
	('PK','Pakistan','Islamic Republic of Pakistan','PAK','586','yes','92','.pk'),
	('PW','Palau','Republic of Palau','PLW','585','yes','680','.pw'),
	('PS','Palestine','State of Palestine (or Occupied Palestinian Territory)','PSE','275','some','970','.ps'),
	('PA','Panama','Republic of Panama','PAN','591','yes','507','.pa'),
	('PG','Papua New Guinea','Independent State of Papua New Guinea','PNG','598','yes','675','.pg'),
	('PY','Paraguay','Republic of Paraguay','PRY','600','yes','595','.py'),
	('PE','Peru','Republic of Peru','PER','604','yes','51','.pe'),
	('PH','Phillipines','Republic of the Philippines','PHL','608','yes','63','.ph'),
	('PN','Pitcairn','Pitcairn','PCN','612','no','NONE','.pn'),
	('PL','Poland','Republic of Poland','POL','616','yes','48','.pl'),
	('PT','Portugal','Portuguese Republic','PRT','620','yes','351','.pt'),
	('PR','Puerto Rico','Commonwealth of Puerto Rico','PRI','630','no','1+939','.pr'),
	('QA','Qatar','State of Qatar','QAT','634','yes','974','.qa'),
	('RE','Reunion','R&eacute;union','REU','638','no','262','.re'),
	('RO','Romania','Romania','ROU','642','yes','40','.ro'),
	('RU','Russia','Russian Federation','RUS','643','yes','7','.ru'),
	('RW','Rwanda','Republic of Rwanda','RWA','646','yes','250','.rw'),
	('BL','Saint Barthelemy','Saint Barth&eacute;lemy','BLM','652','no','590','.bl'),
	('SH','Saint Helena','Saint Helena, Ascension and Tristan da Cunha','SHN','654','no','290','.sh'),
	('KN','Saint Kitts and Nevis','Federation of Saint Christopher and Nevis','KNA','659','yes','1+869','.kn'),
	('LC','Saint Lucia','Saint Lucia','LCA','662','yes','1+758','.lc'),
	('MF','Saint Martin','Saint Martin','MAF','663','no','590','.mf'),
	('PM','Saint Pierre and Miquelon','Saint Pierre and Miquelon','SPM','666','no','508','.pm'),
	('VC','Saint Vincent and the Grenadines','Saint Vincent and the Grenadines','VCT','670','yes','1+784','.vc'),
	('WS','Samoa','Independent State of Samoa','WSM','882','yes','685','.ws'),
	('SM','San Marino','Republic of San Marino','SMR','674','yes','378','.sm'),
	('ST','Sao Tome and Principe','Democratic Republic of S&atilde;o Tom&eacute; and Pr&iacute;ncipe','STP','678','yes','239','.st'),
	('SA','Saudi Arabia','Kingdom of Saudi Arabia','SAU','682','yes','966','.sa'),
	('SN','Senegal','Republic of Senegal','SEN','686','yes','221','.sn'),
	('RS','Serbia','Republic of Serbia','SRB','688','yes','381','.rs'),
	('SC','Seychelles','Republic of Seychelles','SYC','690','yes','248','.sc'),
	('SL','Sierra Leone','Republic of Sierra Leone','SLE','694','yes','232','.sl'),
	('SG','Singapore','Republic of Singapore','SGP','702','yes','65','.sg'),
	('SX','Sint Maarten','Sint Maarten','SXM','534','no','1+721','.sx'),
	('SK','Slovakia','Slovak Republic','SVK','703','yes','421','.sk'),
	('SI','Slovenia','Republic of Slovenia','SVN','705','yes','386','.si'),
	('SB','Solomon Islands','Solomon Islands','SLB','090','yes','677','.sb'),
	('SO','Somalia','Somali Republic','SOM','706','yes','252','.so'),
	('ZA','South Africa','Republic of South Africa','ZAF','710','yes','27','.za'),
	('GS','South Georgia and the South Sandwich Islands','South Georgia and the South Sandwich Islands','SGS','239','no','500','.gs'),
	('KR','South Korea','Republic of Korea','KOR','410','yes','82','.kr'),
	('SS','South Sudan','Republic of South Sudan','SSD','728','yes','211','.ss'),
	('ES','Spain','Kingdom of Spain','ESP','724','yes','34','.es'),
	('LK','Sri Lanka','Democratic Socialist Republic of Sri Lanka','LKA','144','yes','94','.lk'),
	('SD','Sudan','Republic of the Sudan','SDN','729','yes','249','.sd'),
	('SR','Suriname','Republic of Suriname','SUR','740','yes','597','.sr'),
	('SJ','Svalbard and Jan Mayen','Svalbard and Jan Mayen','SJM','744','no','47','.sj'),
	('SZ','Swaziland','Kingdom of Swaziland','SWZ','748','yes','268','.sz'),
	('SE','Sweden','Kingdom of Sweden','SWE','752','yes','46','.se'),
	('CH','Switzerland','Swiss Confederation','CHE','756','yes','41','.ch'),
	('SY','Syria','Syrian Arab Republic','SYR','760','yes','963','.sy'),
	('TW','Taiwan','Republic of China (Taiwan)','TWN','158','former','886','.tw'),
	('TJ','Tajikistan','Republic of Tajikistan','TJK','762','yes','992','.tj'),
	('TZ','Tanzania','United Republic of Tanzania','TZA','834','yes','255','.tz'),
	('TH','Thailand','Kingdom of Thailand','THA','764','yes','66','.th'),
	('TL','Timor-Leste (East Timor)','Democratic Republic of Timor-Leste','TLS','626','yes','670','.tl'),
	('TG','Togo','Togolese Republic','TGO','768','yes','228','.tg'),
	('TK','Tokelau','Tokelau','TKL','772','no','690','.tk'),
	('TO','Tonga','Kingdom of Tonga','TON','776','yes','676','.to'),
	('TT','Trinidad and Tobago','Republic of Trinidad and Tobago','TTO','780','yes','1+868','.tt'),
	('TN','Tunisia','Republic of Tunisia','TUN','788','yes','216','.tn'),
	('TR','Turkey','Republic of Turkey','TUR','792','yes','90','.tr'),
	('TM','Turkmenistan','Turkmenistan','TKM','795','yes','993','.tm'),
	('TC','Turks and Caicos Islands','Turks and Caicos Islands','TCA','796','no','1+649','.tc'),
	('TV','Tuvalu','Tuvalu','TUV','798','yes','688','.tv'),
	('UG','Uganda','Republic of Uganda','UGA','800','yes','256','.ug'),
	('UA','Ukraine','Ukraine','UKR','804','yes','380','.ua'),
	('AE','United Arab Emirates','United Arab Emirates','ARE','784','yes','971','.ae'),
	('GB','United Kingdom','United Kingdom of Great Britain and Nothern Ireland','GBR','826','yes','44','.uk'),
	('US','United States','United States of America','USA','840','yes','1','.us'),
	('UM','United States Minor Outlying Islands','United States Minor Outlying Islands','UMI','581','no','NONE','NONE'),
	('UY','Uruguay','Eastern Republic of Uruguay','URY','858','yes','598','.uy'),
	('UZ','Uzbekistan','Republic of Uzbekistan','UZB','860','yes','998','.uz'),
	('VU','Vanuatu','Republic of Vanuatu','VUT','548','yes','678','.vu'),
	('VA','Vatican City','State of the Vatican City','VAT','336','no','39','.va'),
	('VE','Venezuela','Bolivarian Republic of Venezuela','VEN','862','yes','58','.ve'),
	('VN','Vietnam','Socialist Republic of Vietnam','VNM','704','yes','84','.vn'),
	('VG','Virgin Islands, British','British Virgin Islands','VGB','092','no','1+284','.vg'),
	('VI','Virgin Islands, US','Virgin Islands of the United States','VIR','850','no','1+340','.vi'),
	('WF','Wallis and Futuna','Wallis and Futuna','WLF','876','no','681','.wf'),
	('EH','Western Sahara','Western Sahara','ESH','732','no','212','.eh'),
	('YE','Yemen','Republic of Yemen','YEM','887','yes','967','.ye'),
	('ZM','Zambia','Republic of Zambia','ZMB','894','yes','260','.zm'),
	('ZW','Zimbabwe','Republic of Zimbabwe','ZWE','716','yes','263','.zw')

	DELETE FROM [dbo].[Certificates]

	INSERT INTO [dbo].[Certificates] 
	(
		[UserId],
		[Code],
		[Status]
	)
	VALUES
	(NULL, 'UHK9yFhgWvqAQjUFNzwZSFnO', 'waiting'),
	(NULL, 'OY1Cu82A3RblzAmhEBZtpPC9', 'waiting'),
	(NULL, '2cCdpzmpf2xSnqGs8dAy9oAn', 'waiting'),
	(NULL, 'XXmjq86xYNEmaoXxOR9limaA', 'waiting'),
	(NULL, 'tkCieZuSUepNBYMlOuF1vEOX', 'waiting'),
	(NULL, 'suTx20EfphKVTz9NPSBrSviL', 'waiting'),
	(NULL, 'XopCVa7vCQ9qSqFJNPyADJxf', 'waiting'),
	(NULL, 'w6widn7E01yDiPFtfeTfloNV', 'waiting'),
	(NULL, 'HvKBbmXx6gkV1kPnmUo3CF3H', 'waiting'),
	(NULL, '71z5ugNnsaklZylrhBvZisWe', 'waiting'),
	(NULL, '9p01SOynbB8BNNv9JZ4azCet', 'waiting'),
	(NULL, '9Mj9S00qXavsEVkyxk3unVfp', 'waiting'),
	(NULL, 'Mwr00VqwzYxL3q7we2FEkYV9', 'waiting'),
	(NULL, 'ergvzi22S9iEQaW31AQSW2Kv', 'waiting'),
	(NULL, 'OrRvMBO4MViXroQelccdOKSL', 'waiting'),
	(NULL, '9mjHXkheO8v5px5MNV55H6Sj', 'waiting'),
	(NULL, '2d2Ng3EHgklIOkRCpK2N90oF', 'waiting'),
	(NULL, 'NRouTDxQ31VfP0ZQFyZHbkAL', 'waiting'),
	(NULL, 'Aq6VnCRns908nJd6e28yz4yZ', 'waiting'),
	(NULL, '2msMfNImSZ4trsVjCzJsP3qI', 'waiting'),
	(NULL, 'gIffoVG5h0Tmmwqp23wuedEL', 'waiting'),
	(NULL, 'TyBkIHGVI9fKuUrLgg5O8jyk', 'waiting'),
	(NULL, '7PPxiqFTejPArOuSstzxXTiv', 'waiting'),
	(NULL, 'R5tYDFe9MdoLABrFpamH5IcB', 'waiting'),
	(NULL, 'moaNCOvmTClnaI1dV0YMGzfe', 'waiting'),
	(NULL, 'VlApVirlBFqWHM6ceq5DBN9D', 'waiting'),
	(NULL, '20trf6YqlsOVgljlbM9ejNgT', 'waiting'),
	(NULL, 'cZSJFyxu1d0DjgJB7aDdG4ws', 'waiting'),
	(NULL, 'luuazLW4z61UAymyGPujhGdQ', 'waiting'),
	(NULL, 'lNdIMqFgG4awl2obyVKjjm21', 'waiting'),
	(NULL, 'oVeXUVVzNG7TUm2OKfMLCVkk', 'waiting'),
	(NULL, 'ymBC1vCQ1EBX6Jmw6oqruyGS', 'waiting'),
	(NULL, 'ZkSEK5QD6T8iOHYlkJdEQf0z', 'waiting'),
	(NULL, 'a0WpPZkZ0j4snJowoXfCpOif', 'waiting'),
	(NULL, 'JYztkpAVLnqPwXJ95OHxZ7Hn', 'waiting'),
	(NULL, 't1ZBZEksMHNVImuAhqF3M1Y7', 'waiting'),
	(NULL, 'KmUQYcplJzd2IeEsldne1lwe', 'waiting'),
	(NULL, '3ezpA2ATtNgqZrUHyqUX0u0d', 'waiting'),
	(NULL, 'OLLgPl20CSwJjQmCdD0Lq8Tg', 'waiting'),
	(NULL, '2VTwnKhiMhKT2kOUKwf45CLh', 'waiting'),
	(NULL, 'esyGi6sCURXxEHWT0HIY7a1X', 'waiting'),
	(NULL, 'hxSte2IPZRxqcOE0lkR5b1Np', 'waiting'),
	(NULL, 'YilaeI8vJlQAAZlReb4mx5v2', 'waiting'),
	(NULL, 'pEWVkZhU7qnY6aRUtu7NYwEm', 'waiting'),
	(NULL, '4BxN3b4uzMJVQve8A8pWXmWt', 'waiting'),
	(NULL, '54gmFWOiwypEAa0JeRr0il6E', 'waiting'),
	(NULL, '39Tk6s5pxEi6dbguw4GgfW53', 'waiting'),
	(NULL, 'URheqWLv3w23IdYS9THRdh06', 'waiting'),
	(NULL, 'pv6juu58eqdd9BUaY4uLMqFy', 'waiting'),
	(NULL, 'N1qlrZsRrLitAJ6QRrVqRN18', 'waiting'),
	(NULL, 'CmhI59FrOTfzipiaIDVkByVe', 'waiting'),
	(NULL, 'mBDtoLL5G18zNjor4YulgYDb', 'waiting'),
	(NULL, 'nll7AcR2Lj8gvzF7dVziuGW1', 'waiting'),
	(NULL, 'R6RynqZhZ2t77WMuTyRkq0Ex', 'waiting'),
	(NULL, 'igQZxqmBzVAi6xJDRKYUvWmI', 'waiting'),
	(NULL, 'TR9PRaTkdMV4QChXzXxhJNFF', 'waiting'),
	(NULL, 'K6axt8AL8FAfsrXMV8vCUb9n', 'waiting'),
	(NULL, 'W4YkkvbdnvYdRt2boX6qmbTQ', 'waiting'),
	(NULL, 'FvyayQktZZVHVdF1HsgPg8nD', 'waiting'),
	(NULL, 'baq0Na771kd1lCDIRyq757iF', 'waiting'),
	(NULL, 'JnDqCx6x6E4GfrCp4XztmqEa', 'waiting'),
	(NULL, '9t68QgLfjaCuIF5oqyuNYtM1', 'waiting'),
	(NULL, 'C3bgb992zCVp9nzzoV0SqA3o', 'waiting'),
	(NULL, 'jmN6eiTSk3a2GB35xDDi3tGF', 'waiting'),
	(NULL, 'fqh0JzTy8W4j51M7PqtZF7mC', 'waiting'),
	(NULL, 'ohNRAEqIDW0IpH1yyQVEfT2j', 'waiting'),
	(NULL, 'BilHNOAZIjeRtH1tZj533fYW', 'waiting'),
	(NULL, '8ofgpzX9pURQYBzCeVUwRL2h', 'waiting'),
	(NULL, 'MIBadqUR9t6nYE6y1d7jQ6S0', 'waiting'),
	(NULL, 'V8SROhFmb2V4kYGlHLk3OjMV', 'waiting'),
	(NULL, 'AjOf1SRx54UKhLqCRXgTyT51', 'waiting'),
	(NULL, 'GPvAmOgzuPpKm01cPrMSTE2j', 'waiting'),
	(NULL, 'UU9nG8ahEpSnMS7GgTSTkCRP', 'waiting'),
	(NULL, 'dcork2Qm8ZZ9qb1mSFcosPyf', 'waiting'),
	(NULL, 'jPdecOQ5MbkOBsYkWVzxSAsL', 'waiting'),
	(NULL, 'bLZ4PzsT27WQNHgenYS73tNC', 'waiting'),
	(NULL, 'ulw7zAWEd1FwSa04HJMniu6b', 'waiting'),
	(NULL, 'CZyqcyhhk143kLLxqJ8n2Yjd', 'waiting'),
	(NULL, 'USwnMOQ0sVkmMuuzNvXExgc6', 'waiting'),
	(NULL, 'IXX8UgOCX8RlWlx4Zgdq2mKb', 'waiting'),
	(NULL, 'TmWPk4mR1Hb7gKM3VrqNcTeA', 'waiting'),
	(NULL, 'yGd89lgdMvZVuD9hvRGeC72C', 'waiting'),
	(NULL, 'wStpbEZik7kKew6XeWlAOWiv', 'waiting'),
	(NULL, '57gjHGq5wlKPgu1c4LZp7AYO', 'waiting'),
	(NULL, 'EmPlAQje9bdVvR8WAQeh3JcM', 'waiting'),
	(NULL, 'iRv4cRXTb02Xf4deAicgnyQj', 'waiting'),
	(NULL, '3H0JiiuFbm3jvaTOy9HlVOMl', 'waiting'),
	(NULL, 'bhKTeKU3RqpD3Wnks0opuRcH', 'waiting'),
	(NULL, 'dDZXVSLkJ2Kla8B6SCpXaPrh', 'waiting'),
	(NULL, 'jgK85vP9jfSHt5DaPOvbSEVX', 'waiting'),
	(NULL, 'tFhiamgr2uTFYU8me6zi4VZA', 'waiting'),
	(NULL, '5kVucB5XJRwvKHL6h31mxeqv', 'waiting'),
	(NULL, 'Cvc4Mt2jFwyiCfwOuKJPInme', 'waiting'),
	(NULL, 'fqQoC4TBmC7QErFD1VOJFEsN', 'waiting'),
	(NULL, 'ym57AzYbNqmmdsR3X59xQAyZ', 'waiting'),
	(NULL, 'Zm6ImMmEG7wLO6xItwv1YkSf', 'waiting'),
	(NULL, 'e2l0ubK48IOQQRFtAabgvQII', 'waiting'),
	(NULL, 'W2KM7qzxEITFRR2aBPInI981', 'waiting'),
	(NULL, 'hb2nkUxoGWlJTqCDSIz0Q3iJ', 'waiting'),
	(NULL, 'ZeJ9kUzYT9KbBO2FxcRRmRIU', 'waiting')

END