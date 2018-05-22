using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab.Elecsys2010
{
    class TestParams
    {
        public TestParams()
        {
        }

        public static string getTestCode(int testNo)
        {
            switch(testNo)
            {
                case 010:
                case 011:
                case 012:
                    return "TSH";
                    break;
                case 020:
                case 021:
                case 022:
                    return "T4";
                    break;
                case 030:
                case 031:
                case 032:
                    return "FT4";
                    break;
                case 040:
                case 041:
                case 042:
                    return "T-UP";
                    break;
                case 050:
                case 051:
                case 052:
                    return "T3";
                    break;
                case 060:
                case 061:
                case 062:
                    return "FT3";
                    break;
                case 100:
                case 101:
                    return "E2";
                    break;
                case 110:
                case 111:
                    return "TESTO";
                    break;
                case 120:
                case 121:
                    return "PROG";
                    break;
                case 130:
                case 131:
                    return "PRL";
                    break;
                case 140:
                case 141:
                    return "LH";
                    break;
                case 150:
                case 151:
                    return "FSH";
                    break;
                case 160:
                case 161:
                    return "CORT";
                    break;
                case 170:
                case 171:
                case 172:
                    return "HCGSTAT";
                    break;
                case 180:
                case 181:
                case 182:
                    return "HCG";
                    break;
                case 200:
                case 201:
                case 202: 
                    return "TNTSTAT";
                    break;
                case 210:
                case 211:
                case 212: 
                    return "CKMBSTAT";
                    break;
                case 220:
                case 221:
                case 222: 
                    return "TN-T";
                    break;
                case 230:
                case 231:
                case 232:               
                    return "CK-MB";
                    break;
                case 240:
                case 241:
                    return "MYO";
                    break;
                case 250:
                case 251:
                    return "MYO-STAT";
                    break;
                case 300:
                case 301:
                    return "CEA";
                    break;
                case 310:
                case 311:
                    return "AFP";
                    break;
                case 320:
                case 321:
                    return "PSA";
                    break;
                case 330:
                case 331:
                    return "CA 15-3";
                    break;
                case 340:
                case 341:
                    return "CA 125";
                    break;
                case 350:
                case 351:
                    return "CA 19-9";
                    break;
                case 360:
                case 361:
                    return "CA 72-4";
                    break;
                case 370:
                case 371:
                    return "CYFRA";
                    break;
                case 380:
                case 381:
                    return "FERR";
                    break;
                case 390:
                case 391:
                    return "FPSA";
                    break;
                case 400:
                case 401:
                    return "HBSAG";
                    break;
                case 410:
                case 411:
                    return "A-HBS";
                    break;
                case 420:
                case 421:
                    return "A-HCV";
                    break;
                case 430:
                case 431:
                    return "A-HBE";
                    break;
                case 440:
                case 441:
                    return "HBEAG";
                    break;
                case 450:
                case 451:
                    return "A-HBC";
                    break;
                case 460:
                case 461:
                    return "A-HBCIGM";
                    break;
                case 470:
                case 471:
                    return "A-HAV";
                    break;
                case 480:
                case 481:
                    return "A-HAVIGM";
                    break;
                case 490:
                case 491:
                    return "A-HIV";
                    break;
                case 500:
                case 501:
                    return "HIVAG";
                    break;
                case 510:
                case 511:
                    return "APS4";
                    break;
                case 520:
                case 521:
                    return "A-TOXIGG";
                    break;
                case 530:
                case 531:
                    return "A-TOXIGM";
                    break;
                case 540:
                case 541:
                    return "A-RUBIGG";
                    break;
                case 550:
                case 551:
                    return "A-RUBIGM";
                    break;
                case 560:
                case 561:
                    return "HIVCOM";
                    break;
                case 570:
                case 571:
                    return "A-HGVENV";
                    break;
                case 580:
                case 581:
                    return "A-HELICO";
                    break;
                case 590:
                case 591:
                    return "B12";
                    break;
                case 610:
                case 611:
                    return "FOL";
                    break;
                case 620:
                case 621:
                    return "DIG";
                    break;
                case 630:
                case 631:
                    return "IGE";
                    break;
                case 640:
                case 641:
                    return "HBA1C";
                    break;
                case 650:
                case 651:
                    return "INSULIN";
                    break;
                case 660:
                case 661:
                    return "OSTEOC";
                    break;
                case 670:
                case 671:
                    return "CROSSL";
                    break;
                case 680:
                case 681:
                    return "PTH";
                    break;
                case 690:
                case 691:
                    return "CYCLO-A";
                    break;
                case 700:
                case 701:
                    return "TG";
                    break;
                case 710:
                case 711:
                    return "A-TG";
                    break;
                case 720:
                case 721:
                    return "A-TPO";
                    break;
                case 730:
                case 731:
                    return "A-TSHR";
                    break;
                case 740:
                case 741:
                    return "DHEA-S";
                    break;
                case 750:
                case 751:
                    return "SHBG";
                    break;
                case 760:
                case 761:
                    return "HCG-BETA";
                    break;
                case 770:
                case 771:
                    return "NSE";
                    break;
                case 790:
                    return "HBsAg-QN";
                    break;
                case 900:
                case 901:
                    return "HBsAg-II";
                    break;
                default:
                    return "XXX";
            }
        }
    }
}
