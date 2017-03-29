using FaceSysByMvvm;
using FaceSysByMvvm.Common;
using ThriftService;
using FaceSysByMvvm.Views;
using FaceSysByMvvm.Views.ChannelManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceSysClient.ClassPool
{
    class UIServerInter : UIServer.Iface
    {
        public WriteLog _WriteLog = new WriteLog();
        public MyCapFaceLogWithImg _MyCapFaceLogWithImg;
        public IdentifyResults _IdentifyResults;
        List<string> listQueryDefFaceObjType = new List<string>();
        ThirftService thirft = new ThirftService();

        #region Old Interface
        public int UpdateRealtimeCap(RealtimeCapInfo info, string channelName)
        {
            try
            {
                if (Login.ClientType == "1")
                {
                    if (!channelName.Contains(FaceSysByMvvm.Login.ClientAreaName))
                    {
                        return -1;
                    }
                }
                //接收服务器附送过来的实时照片
                if (_MyCapFaceLogWithImg != null)
                    _MyCapFaceLogWithImg = null;
                _MyCapFaceLogWithImg = new MyCapFaceLogWithImg();
                _MyCapFaceLogWithImg.ID = info.Id;// 抓拍id
                //string strChannelId = info.Channel.Substring(0, 6) + "\r\n";
                //strChannelId += info.Channel.Substring(0, 6);
                _MyCapFaceLogWithImg.ChannelID = info.Channel;// 通道id
                _MyCapFaceLogWithImg.ChannelName = channelName;
                long _longtime = info.Time;
                DateTime s = new DateTime(1970, 1, 1);
                s = s.AddSeconds(_longtime);
                _MyCapFaceLogWithImg.time = s.ToString("yyyy/MM/dd HH:mm:ss");

                ChannelManage.CapimageByteRealtimeCapInfo = info.Image;
                ChannelManage._MyCapFaceLogWithImg = null;
                ChannelManage._MyCapFaceLogWithImg = _MyCapFaceLogWithImg;
                ChannelManage.ResetServerRealtimeCapInfo.Set();
                info = null;

                return 0;
            }
            catch (Exception ex)
            {
                _WriteLog.WriteToLog("UpdateRealtimeCap", ex);
                return -1;
            }
        }

        public int UpdateRealtimeCmp(RealtimeCmpInfo info, string channelName)
        {
            //return 1;
            try
            {
                if (Login.ClientType == "1")
                {
                    if (!channelName.StartsWith("##"))
                    {
                        return -1;
                    }
                }
                //显示在界面上的结果
                if (_IdentifyResults != null)
                    _IdentifyResults = null;
                _IdentifyResults = new IdentifyResults();
                _IdentifyResults.ID = info.CapID;
                if (info.CapID == null || info.CapID == "")
                {
                    MB_MODULES.Views.MyMessage.showYes("抓拍照片ID为空");
                }
                _IdentifyResults.RegID = info.ObjID;
                if (info.ObjID == null || info.ObjID == "")
                {
                    //MB_MODULES.Views.MyMessage.showYes("抓拍照片ID为空");
                    MB_MODULES.Views.MyMessage.showYes("抓拍照片ID为空");
                }
                long _longtime = info.Time;
                DateTime s = new DateTime(1970, 1, 1);
                s = s.AddSeconds(_longtime);
                // 抓拍照片 
                ChannelManage.CapimageByteRealtimeCmpInfo = info.CapImg;
                //得到主照片
                ChannelManage.CmpimageByteRealtimeCmpInfo = info.ObjImg;
                //获得主照片这侧信息
                StringBuilder strRegster = new StringBuilder();
                //注册名称
                strRegster.Append(info.Name + "\r\n");
                //获得通道名称
                string ChannelName = channelName;
                _IdentifyResults.ChannelName = channelName;
                strRegster.Append(ChannelName.Replace("##", "").Replace("@", "") + "\r\n");
                //抓拍时间
                int nIndexS = s.ToString().IndexOf(" ");
                strRegster.Append(s.ToString().Substring(0, nIndexS) + "\r\n");
                int nIndexS1 = s.ToString().Length - nIndexS;
                strRegster.Append(s.ToString().Substring(nIndexS + 1, nIndexS1 - 1) + "\r\n");
                //注册类型
                string type = "";
                foreach (var basicinfo in AppConfigs.DefFaceObjType)
                {
                    if (basicinfo.Type == info.Type)
                    {
                        type = basicinfo.Description; // 类型  
                    }
                }
                strRegster.Append(type + "\r\n");
                _IdentifyResults.TemplateType = type;
                //相似度。
                strRegster.Append(info.Score + "\r\n");
                _IdentifyResults.RegInfo = strRegster.ToString();
                _IdentifyResults.Info = info;
                //MainWindow.nInfoType = info.Type;
                Console.WriteLine("推送了一条比对记录");
                ChannelManage._IdentifyResults = _IdentifyResults;
                ChannelManage.ResetServerRealtimeCmpInfo.Set();

                ////弹出报警框
                //AlarmWindow _AlarmWindow = new AlarmWindow();
                //_AlarmWindow.CapimageByteRealtimeCmpInfo = info.CapImg;
                //_AlarmWindow.CmpimageByteRealtimeCmpInfo = info.CapImg;
                //_AlarmWindow.RegInfo = strRegster.ToString();
                //_AlarmWindow.nInfoType = info.Type;

                //_AlarmWindow.Show();

                info = null;
                return 0;
            }
            catch (Exception ex)
            {
                _WriteLog.WriteToLog("UpdateRealtimeCmp", ex);
                return -1;
            }
        }
        #endregion


        public int UpdateRealtimeCapLBS(RealtimeCapInfoLBS info)
        {
            try
            {
                if (Login.ClientType == "1")
                {
                    if (!info.Channelname.Contains(FaceSysByMvvm.Login.ClientAreaName))
                    {
                        return -1;
                    }
                }
                //接收服务器附送过来的实时照片
                if (_MyCapFaceLogWithImg != null)
                    _MyCapFaceLogWithImg = null;
                _MyCapFaceLogWithImg = new MyCapFaceLogWithImg();
                _MyCapFaceLogWithImg.ID = info.Id;// 抓拍id
                //string strChannelId = info.Channel.Substring(0, 6) + "\r\n";
                //strChannelId += info.Channel.Substring(0, 6);
                _MyCapFaceLogWithImg.ChannelID = info.Channel;// 通道id
                _MyCapFaceLogWithImg.ChannelName = info.Channelname;
                _MyCapFaceLogWithImg.Longitude = info.Longitude;
                _MyCapFaceLogWithImg.Latitude = info.Latitude;
                _MyCapFaceLogWithImg.Score = info.Score;
                _MyCapFaceLogWithImg.Address = info.Address;

                long _longtime = info.Time;
                DateTime s = new DateTime(1970, 1, 1);
                s = s.AddSeconds(_longtime);
                _MyCapFaceLogWithImg.time = s.ToString("yyyy/MM/dd HH:mm:ss");

                ChannelManage.CapimageByteRealtimeCapInfo = info.Image;
                ChannelManage._MyCapFaceLogWithImg = null;
                ChannelManage._MyCapFaceLogWithImg = _MyCapFaceLogWithImg;
                ChannelManage.ResetServerRealtimeCapInfo.Set();
                info = null;

                return 0;
            }
            catch (Exception ex)
            {
                _WriteLog.WriteToLog("UpdateRealtimeCapLBS", ex);
                return -1;
            }
        }

        /// <summary>
        /// 实时更新比对结果，显示到结果列表中
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int UpdateRealtimeCmpLBS(RealtimeCmpInfoLBS info)
        {
            //return 1;
            try
            {
                if (Login.ClientType == "1")
                {
                    if (!info.Channelname.StartsWith("##"))
                    {
                        return -1;
                    }
                }
                //显示在界面上的结果
                if (_IdentifyResults != null)
                    _IdentifyResults = null;
                _IdentifyResults = new IdentifyResults();
                _IdentifyResults.ID = info.CapID;
                if (info.CapID == null || info.CapID == "")
                {
                    MB_MODULES.Views.MyMessage.showYes("抓拍照片ID为空");
                }
                _IdentifyResults.RegID = info.ObjID;
                if (info.ObjID == null || info.ObjID == "")
                {
                    MB_MODULES.Views.MyMessage.showYes("抓拍照片ID为空");
                }
                long _longtime = info.Time;
                DateTime s = new DateTime(1970, 1, 1);
                s = s.AddSeconds(_longtime);
                // 抓拍照片 
                ChannelManage.CapimageByteRealtimeCmpInfo = info.CapImg;
                //得到主照片
                ChannelManage.CmpimageByteRealtimeCmpInfo = info.ObjImg;
                //获得主照片这侧信息
                StringBuilder strRegster = new StringBuilder();
                //注册名称
                strRegster.Append(info.Name + "\r\n");
                //获得通道名称
                string ChannelName = info.Channelname;
                _IdentifyResults.ChannelName = info.Channelname;
                strRegster.Append(ChannelName.Replace("##", "").Replace("@", "") + "\r\n");
                //抓拍时间
                int nIndexS = s.ToString().IndexOf(" ");
                strRegster.Append(s.ToString().Substring(0, nIndexS) + "\r\n");
                int nIndexS1 = s.ToString().Length - nIndexS;
                strRegster.Append(s.ToString().Substring(nIndexS + 1, nIndexS1 - 1) + "\r\n");
                //注册类型
                string type = "";
                foreach (var basicinfo in AppConfigs.DefFaceObjType)
                {
                    if (basicinfo.Type == info.Type)
                    {
                        type = basicinfo.Description; // 类型  
                    }
                }
                strRegster.Append(type + "\r\n");
                _IdentifyResults.TemplateType = type;
                //相似度。
                strRegster.Append(info.Score + "\r\n");
                _IdentifyResults.RegInfo = strRegster.ToString();
                _IdentifyResults.NewRci = info;
                //MainWindow.nInfoType = info.Type;
                Console.WriteLine("推送了一条比对记录");
                ChannelManage._IdentifyResults = _IdentifyResults;
                ChannelManage.ResetServerRealtimeCmpInfo.Set();

                ////弹出报警框
                //AlarmWindow _AlarmWindow = new AlarmWindow();
                //_AlarmWindow.CapimageByteRealtimeCmpInfo = info.CapImg;
                //_AlarmWindow.CmpimageByteRealtimeCmpInfo = info.CapImg;
                //_AlarmWindow.RegInfo = strRegster.ToString();
                //_AlarmWindow.nInfoType = info.Type;
                //_AlarmWindow.Show();

                info = null;
                return 0;
            }
            catch (Exception ex)
            {
                _WriteLog.WriteToLog("UpdateRealtimeCmp", ex);
                return -1;
            }
        }

        public int UpdateRealtimeCmpEntertainment(CmpFaceLogDS cfg)
        {

            return 1;

        }
    }
}
