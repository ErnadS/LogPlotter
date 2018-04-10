using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLog.graph;

namespace utility
{
    public class NmeaSearchParams
    {
        public String strMsgType;
        public int nmeaParamInd;
        public String strParamName;
        public String strTalkerId;
        public String validatorString;
        public int validatorIndex;

        public List<GraphPoint> listValues = new List<GraphPoint>();

        public NmeaSearchParams(String strMsgType, int nmeaParamInd, String strParamName)
        {
            this.strMsgType = strMsgType;  // last 3 chars in NMEA name
            this.nmeaParamInd = nmeaParamInd;
            this.strParamName = strParamName;
            strTalkerId = "";

            validatorString = "";
            validatorIndex = -1;
        }

        public NmeaSearchParams(String strMsgType, String validatorString, int validatorIndex, int nmeaParamInd, String strParamName)
        {
            this.strMsgType     = strMsgType;  // last 3 chars in NMEA name
            this.nmeaParamInd   = nmeaParamInd;
            this.strParamName   = strParamName;
            strTalkerId = "";
            this.validatorString = validatorString;
            this.validatorIndex = validatorIndex;
        }

        public NmeaSearchParams(String strTalkerId, String strMsgType, int nmeaParamInd, String strParamName)
        {
            this.strMsgType = strMsgType;  // last 3 chars in NMEA name
            this.nmeaParamInd = nmeaParamInd;
            this.strParamName = strParamName;
            this.strTalkerId = strTalkerId;
        }

        public NmeaSearchParams(String strTalkerId, String validatorString, int validatorIndex, String strMsgType, int nmeaParamInd, String strParamName)
        {
            this.strMsgType = strMsgType;  // last 3 chars in NMEA name
            this.nmeaParamInd = nmeaParamInd;
            this.strParamName = strParamName;
            this.strTalkerId = strTalkerId;

            this.validatorString = validatorString;
            this.validatorIndex = validatorIndex;
        }

        public void addPoint(UInt32 noOfDAT, float newValue)
        {
            listValues.Add(new GraphPoint(noOfDAT, newValue));
        }
    }
}