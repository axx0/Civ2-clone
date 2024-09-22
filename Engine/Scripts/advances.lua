
--0 - Advanced Flight,    4,-2,  Rad, Too, 3, 4    ; AFl
--1 - Alphabet,           5, 1,  nil, nil, 0, 3    ; Alp
--2 - Amphibious Warfare, 3,-2,  Nav, Tac, 3, 0    ; Amp
--3 - Astronomy,          4, 1,  Mys, Mat, 1, 3    ; Ast
--4 - Atomic Theory,      4,-1,  ToG, Phy, 2, 3    ; Ato
--5 - Automobile,         6,-1,  Cmb, Stl, 3, 4    ; Aut
civ.getTech(5).AddEffect(civ.core.Effects.PopulationPollutionModifier, 1)
civ.getTech(5).AddEffect(civ.core.Effects.Epoch, 3)
--6 - Banking,            4, 1,  Tra, Rep, 1, 1    ; Ban
--7 - Bridge Building,    4, 0,  Iro, Cst, 0, 4    ; Bri
--8 - Bronze Working,     6,-1,  nil, nil, 0, 4    ; Bro
--9 - Ceremonial Burial,  5, 0,  nil, nil, 0, 2    ; Cer
--10 - Chemistry,          5,-1,  Uni, Med, 1, 3    ; Che
--11 - Chivalry,           4,-2,  Feu, Hor, 1, 0    ; Chi
--12 - Code of Laws,       4, 1,  Alp, nil, 0, 2    ; CoL
--13 - Combined Arms,      5,-1,  Mob, AFl, 3, 0    ; CA
--14 - Combustion,         5,-1,  Ref, Exp, 2, 4    ; Cmb
--15 - Communism,          5, 0,  Phi, Ind, 2, 2    ; Cmn
--16 - Computers,          4, 1,  Min, MP,  3, 4    ; Cmp
--17 - Conscription,       7,-1,  Dem, Met, 2, 0    ; Csc
--18 - Construction,       4, 0,  Mas, Cur, 0, 4    ; Cst
--19 - The Corporation,    4, 0,  Ind, Eco, 2, 1    ; Cor
--20 - Currency,           4, 1,  Bro, nil, 0, 1    ; Cur
--21 - Democracy,          5, 1,  Ban, Inv, 2, 2    ; Dem
--22 - Economics,          4, 1,  Uni, Ban, 2, 1    ; Eco
--23 - Electricity,        4, 0,  Met, Mag, 2, 4    ; E1
--24 - Electronics,        4, 1,  E1,  Cor, 3, 4    ; E2
civ.getTech(24).AddEffect(civ.core.Effects.Epoch, 3)
--25 - Engineering,        4, 0,  Whe, Cst, 0, 4    ; Eng
--26 - Environmentalism,   3, 1,  Rec, SFl, 3, 2    ; Env
civ.getTech(26).AddEffect(civ.core.Effects.PopulationPollutionModifier, -1)
--27 - Espionage,          2,-1,  Cmn, Dem, 3, 0    ; Esp
--28 - Explosives,         5, 0,  Gun, Che, 2, 4    ; Exp
--29 - Feudalism,          4,-1,  War, Mon, 0, 0    ; Feu
--30 - Flight,             4,-1,  Cmb, ToG, 2, 4    ; Fli
--31 - Fundamentalism,     3,-2,  MT,  Csc, 2, 2    ; Fun
--32 - Fusion Power,       3, 0,  NP,  Sup, 3, 3    ; FP
--33 - Genetic Engineering,3, 2,  Med, Cor, 3, 3    ; Gen
--34 - Guerrilla Warfare,  4, 1,  Cmn, Tac, 3, 0    ; Gue
--35 - Gunpowder,          8,-2,  Inv, Iro, 1, 0    ; Gun
--36 - Horseback Riding,   4,-1,  nil, nil, 0, 0    ; Hor
--37 - Industrialization,  6, 0,  RR,  Ban, 2, 1    ; Ind
civ.getTech(37).AddEffect(civ.core.Effects.PopulationPollutionModifier, 2)
civ.getTech(37).AddEffect(civ.core.Effects.Epoch, 2)
--38 - Invention,          6, 0,  Eng, Lit, 1, 4    ; Inv
civ.getTech(38).AddEffect(civ.core.Effects.Epoch, 1)
--39 - Iron Working,       5,-1,  Bro, War, 0, 4    ; Iro
--40 - Labor Union,        4,-1,  MP,  Gue, 3, 2    ; Lab
--41 - The Laser,          4, 0,  NP,  MP,  3, 3    ; Las
--42 - Leadership,         5,-1,  Chi, Gun, 1, 0    ; Ldr
--43 - Literacy,           5, 2,  Wri, CoL, 0, 3    ; Lit
--44 - Machine Tools,      4,-2,  Stl, Tac, 2, 4    ; Too
--45 - Magnetism,          4,-1,  Phy, Iro, 1, 3    ; Mag
--46 - Map Making,         6,-1,  Alp, nil, 0, 1    ; Map
--47 - Masonry,            4, 1,  nil, nil, 0, 4    ; Mas
--48 - Mass Production,    5, 0,  Aut, Cor, 3, 4    ; MP
civ.getTech(48).AddEffect(civ.core.Effects.PopulationPollutionModifier, 1)
--49 - Mathematics,        4,-1,  Alp, Mas, 0, 3    ; Mat
--50 - Medicine,           4, 0,  Phi, Tra, 1, 1    ; Med
--51 - Metallurgy,         6,-2,  Gun, Uni, 1, 0    ; Met
--52 - Miniaturization,    4, 1,  Too, E2,  3, 4    ; Min
--53 - Mobile Warfare,     8,-1,  Aut, Tac, 3, 0    ; Mob
--54 - Monarchy,           5, 1,  Cer, CoL, 0, 2    ; Mon
--55 - Monotheism,         5, 1,  Phi, PT,  1, 2    ; MT
--56 - Mysticism,          4, 0,  Cer, nil, 0, 2    ; Mys
--57 - Navigation,         6,-1,  Sea, Ast, 1, 1    ; Nav
--58 - Nuclear Fission,    6,-2,  Ato, MP,  3, 3    ; NF
--59 - Nuclear Power,      3, 0,  NF,  E2,  3, 3    ; NP
--60 - Philosophy,         6, 1,  Mys, Lit, 1, 2    ; Phi
civ.getTech(60).AddEffect(civ.core.Effects.Epoch, 1)
--61 - Physics,            4,-1,  Nav, Lit, 1, 3    ; Phy
--62 - Plastics,           4, 1,  Ref, SFl, 3, 4    ; Pla
civ.getTech(62).AddEffect(civ.core.Effects.PopulationPollutionModifier, 1)
--63 - Plumbing,           4, 0,  no,  no,  1, 4    ; Plu  (Cst & Pot)
--64 - Polytheism,         4, 0,  Cer, Hor, 0, 2    ; PT
--65 - Pottery,            4, 1,  nil, nil, 0, 1    ; Pot
--66 - Radio,              5,-1,  Fli, E1,  3, 4    ; Rad
--67 - Railroad,           6, 0,  SE, Bri,  2, 1    ; RR
--68 - Recycling,          2, 1,  MP, Dem,  3, 2    ; Rec
--69 - Refining,           4, 0,  Che, Cor, 2, 4    ; Ref
--70 - Refrigeration,      3, 1,  E1,  San, 3, 1    ; Rfg
--71 - The Republic,       5, 1,  CoL, Lit, 0, 2    ; Rep
--72 - Robotics,           5,-2,  Cmp, Mob, 3, 0    ; Rob
--73 - Rocketry,           6,-2,  AFl, E2,  3, 0    ; Roc
--74 - Sanitation,         4, 2,  Med, Eng, 2, 1    ; San
civ.getTech(74).AddEffect(civ.core.Effects.PopulationPollutionModifier, -1)
--75 - Seafaring,          4, 1,  Map, Pot, 0, 1    ; Sea
--76 - Space Flight,       4, 1,  Cmp, Roc, 3, 3    ; SFl
--77 - Stealth,            3,-2,  Sup, Rob, 3, 0    ; Sth
--78 - Steam Engine,       4,-1,  Phy, Inv, 2, 3    ; SE
--79 - Steel,              4,-1,  E1,  Ind, 2, 4    ; Stl
--80 - Superconductor,     4, 1,  Pla, Las, 3, 3    ; Sup
--81 - Tactics,            6,-1,  Csc, Ldr, 2, 0    ; Tac
--82 - Theology,           3, 2,  MT,  Feu, 1, 2    ; The
--83 - Theory of Gravity,  4, 0,  Ast, Uni, 1, 3    ; ToG
--84 - Trade,              4, 2,  Cur, CoL, 0, 1    ; Tra
--85 - University,         5, 1,  Mat, Phi, 1, 3    ; Uni
--86 - Warrior Code,       4,-1,  nil, nil, 0, 0    ; War
--87 - The Wheel,          4,-1,  Hor, nil, 0, 4    ; Whe
--88 - Writing,            4, 2,  Alp, nil, 0, 3    ; Wri
--89 - Future Technology,  1, 0,  FP,  Rec, 3, 3    ; ...
--90 - User Def Tech A,    3, 0,  no,  no,  0, 0    ; U1
--91 - User Def Tech B,    3, 0,  no,  no,  0, 0    ; U2
--92 - User Def Tech C,    3, 0,  no,  no,  0, 0    ; U3
--93 - Extra Advance 1,    3, 0,  no,  no,  0, 0    ; X1
--94 - Extra Advance 2,    3, 0,  no,  no,  0, 0    ; X2
--95 - Extra Advance 3,    3, 0,  no,  no,  0, 0    ; X3
--96 - Extra Advance 4,    3, 0,  no,  no,  0, 0    ; X4
--97 - Extra Advance 5,    3, 0,  no,  no,  0, 0    ; X5
--98 - Extra Advance 6,    3, 0,  no,  no,  0, 0    ; X6
--99 - Extra Advance 7,    3, 0,  no,  no,  0, 0    ; X7