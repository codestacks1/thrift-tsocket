using FaceSysClient.ClassPool;
using System;
using System.Collections.Generic;
using Thrift.Protocol;
using Thrift.Transport;
using static FaceSysByMvvm.ViewModels.CompOfRecords.CompOfRecordsViewModel;
using static FaceSysByMvvm.ViewModels.CaptureRecordQuery.CaptureRecordQueryViewModel;
using FaceSysByMvvm.ViewModels.TemplateManager;
using FaceSysByMvvm.Common;
using System.Windows.Threading;
using FaceSysByMvvm.Model;
using GlobalVar;
using DATA.UTILITIES.Log4Net;
using DATA.CONVERTER.Image;
using THRIFTSERVICES.CommonServices;
using DATA.MODELS.GlobalModels;

namespace ThriftService
{
    public class ThirftService : IThirtfService
    {
        public static string iHost = string.Empty;
        public static int iPort;

        /// <summary>
        /// 心跳
        /// </summary>
        /// <returns></returns>
        public int HearBeat()
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 1000, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, int>(transport, bServerClient.HearBeat, "HearBeat");
        }

        /// <summary>
        /// 登录函数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public bool Login(string ip, int port)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(ip, port, 100, ref bServerClient);

            GlobalVars.Host = ip;
            GlobalVars.Port = port;

            iHost = ToServerInfo.ServerIp = ip;
            port = ToServerInfo.ServerPort = port;

            SocketOpter.GetResult<ThirftService, bool>(transport, null, "");

            return true;
        }


        /// <summary>
        /// 获取所有频道
        /// </summary>
        /// <returns></returns>
        public List<MyChannelCfg> QueryAllChannel()
        {
            List<MyChannelCfg> result = new List<MyChannelCfg>();
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            List<ChannelCfgLBS> ListChannelCfg =
            SocketOpter.GetResult<ThirftService, List<ChannelCfgLBS>>(transport, bServerClient.QueryAllChannelLBS, "QueryAllChannel检出所有通道");

            //todo(暂时不需要) 包装返回类 使其返回需要的类
            foreach (ChannelCfgLBS cc in ListChannelCfg)
            {
                result.Add(new MyChannelCfg().ChannelCfgToMyChannelCfgLBS(cc));
            }
            return result;
        }

        /// <summary>
        /// 获取模版类型
        /// </summary>
        /// <returns></returns>
        public List<string> QueryDefFaceObjType()
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<string>>(
                transport, bServerClient.QueryDefFaceObjType, "QueryDefFaceObjType获取模板类型"
                );
        }

        /// <summary>
        /// 获取模版数量
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="name">模板名称</param>
        /// <param name="type">模板类型</param>
        /// <param name="gender">性别</param>
        /// <param name="bage">起始年龄</param>
        /// <param name="eage">终止年龄</param>
        /// <param name="btime">起始时间</param>
        /// <param name="etime">终止时间</param>
        /// <returns></returns>
        public int QueryCmpRecordTotalCount(CompOfRecordsValue compOfRecordsValue)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);

            return SocketOpter.GetResult<ThirftService, int, string, string, int, int, int, int, long, long>(
                transport,
                bServerClient.QueryCmpRecordTotalCount,
                "QueryCmpRecordTotalCount",
                compOfRecordsValue.ChannelValue,
                compOfRecordsValue.NameValue, compOfRecordsValue.TypeValue, compOfRecordsValue.SexValue, compOfRecordsValue.LittleAgeValue, compOfRecordsValue.OldAgeValue, compOfRecordsValue.StartDayValue, compOfRecordsValue.EndDayValue
                );
        }

        /// <summary>
        /// 查询比对记录数量(分表)
        /// </summary>
        /// <param name="compOfRecordsValue"></param>
        /// <returns></returns>
        public List<SCountInfo> QueryCmpRecordTotalCountH(CompOfRecordsValue compOfRecordsValue)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<SCountInfo>, string, string, int, int, int, int, long, long>(
                  transport,
                  bServerClient.QueryCmpRecordTotalCountHDS,
                  "QueryCmpRecordTotalCountH",
                  compOfRecordsValue.ChannelValue,
                  compOfRecordsValue.NameValue,
                  compOfRecordsValue.TypeValue,
                  compOfRecordsValue.SexValue,
                  compOfRecordsValue.LittleAgeValue,
                  compOfRecordsValue.OldAgeValue,
                  compOfRecordsValue.StartDayValue,
                  compOfRecordsValue.EndDayValue
                  );
        }

        public List<SCountInfo> QueryCmpRecordTotalCountHSX(CompOfRecordsValue compOfRecordsValue, int pflag)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<SCountInfo>, string, string, int, int, int, int, long, long, int>(
                transport,
                bServerClient.QueryCmpRecordTotalCountHSXDS,
                 "QueryCmpRecordTotalCountHSX",
                compOfRecordsValue.ChannelValue, compOfRecordsValue.NameValue,
                   compOfRecordsValue.TypeValue, compOfRecordsValue.SexValue, compOfRecordsValue.LittleAgeValue, compOfRecordsValue.OldAgeValue, compOfRecordsValue.StartDayValue, compOfRecordsValue.EndDayValue, pflag
                );
        }


        /// <summary>
        /// 查询比对(分区域)
        /// </summary>
        /// <param name="compOfRecordsValue"></param>
        /// <param name="pflag"></param>
        /// <returns></returns>
        public List<MyCmpFaceLogWidthImgModel> QueryCmpLogSX(CompOfRecordsValue compOfRecordsValue, int pflag)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            List<CmpFaceLogDS> listCmpFaceLog = SocketOpter.GetResult<ThirftService, List<CmpFaceLogDS>, string, string, int, int, int, int, long, long, int, int, int>(
                transport,
                bServerClient.QueryCmpLogSXDS,
                "QueryCmpLogSX ",
                compOfRecordsValue.ChannelValue, compOfRecordsValue.NameValue,
                   compOfRecordsValue.TypeValue, compOfRecordsValue.SexValue,
                   compOfRecordsValue.LittleAgeValue, compOfRecordsValue.OldAgeValue,
                   compOfRecordsValue.StartDayValue, compOfRecordsValue.EndDayValue,
                   compOfRecordsValue.PageStartRow, compOfRecordsValue.PageSize, pflag
                );

            List<MyCmpFaceLogWidthImgModel> listMyCmpFaceLogWidthImg = new List<MyCmpFaceLogWidthImgModel>();
            try
            {
                //比对结果            
                for (int i = listCmpFaceLog.Count - 1; i >= 0; i--)
                {
                    MyCmpFaceLogWidthImgModel _MyCmpFaceLogWidthImg = new MyCmpFaceLogWidthImgModel();
                    //获得序号
                    _MyCmpFaceLogWidthImg.num = compOfRecordsValue.MaxDataCount - compOfRecordsValue.PageStartRow - i;
                    _MyCmpFaceLogWidthImg.ID = listCmpFaceLog[i].ID;// 标识ID

                    //获得通道名称 
                    foreach (MyChannelCfg mcc in QueryAllChannel())
                    {
                        if (listCmpFaceLog[i].Channel == mcc.TcChaneelID)
                        {
                            _MyCmpFaceLogWidthImg.channelName = mcc.Name;
                        }
                    }
                    //_MyCmpFaceLogWidthImg.channel = listCmpFaceLog[i].Channel;// 抓拍通道
                    long longTime = listCmpFaceLog[i].Time;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyCmpFaceLogWidthImg.date = time.ToString("yyyy/MM/dd");
                    _MyCmpFaceLogWidthImg.time = time.ToString("HH:mm:ss");// 抓拍时间

                    Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.SnapImage =
                    ImageConvert.ToBitmapImage(listCmpFaceLog[i].Capimg));//抓拍照片
                    Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.DBImage =
                    ImageConvert.ToBitmapImage(listCmpFaceLog[i].Senceimg));//场景照片

                    if (listCmpFaceLog[i].Ft.Count == 0)
                    {
                        _MyCmpFaceLogWidthImg.T1 = new CompOfRecordTemplate();
                        Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T1.TemplateImage =
                        ImageConvert.ToImageSource("pack://application:,,,/Images/unkonw.jpg"));
                    };

                    for (int j = 0; j < listCmpFaceLog[i].Ft.Count; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                _MyCmpFaceLogWidthImg.T1 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T1.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T1.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T1.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T1.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                _MyCmpFaceLogWidthImg.score = listCmpFaceLog[i].Ft[j].Score;
                                break;
                            case 1:
                                _MyCmpFaceLogWidthImg.T2 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T2.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T2.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T2.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T2.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            case 2:
                                _MyCmpFaceLogWidthImg.T3 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T3.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T3.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T3.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T3.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            case 3:
                                _MyCmpFaceLogWidthImg.T4 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T4.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T4.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T4.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T4.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            case 4:
                                _MyCmpFaceLogWidthImg.T5 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T5.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T5.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T5.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T5.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            default:
                                break;
                        }
                    }

                    listMyCmpFaceLogWidthImg.Add(_MyCmpFaceLogWidthImg);
                }
            }
            catch (Exception)
            {
            }
            return listMyCmpFaceLogWidthImg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compOfRecordsValue"></param>
        /// <returns></returns>
        public List<MyCmpFaceLogWidthImgModel> QueryCmpLog(CompOfRecordsValue compOfRecordsValue)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            List<CmpFaceLogDS> listCmpFaceLog = SocketOpter.GetResult<ThirftService, List<CmpFaceLogDS>, string, string, int, int, int, int, long, long, int, int>(
                transport,
                bServerClient.QueryCmpLogDS,
                "QueryCmpLog ",
                compOfRecordsValue.ChannelValue, compOfRecordsValue.NameValue,
                  compOfRecordsValue.TypeValue, compOfRecordsValue.SexValue,
                  compOfRecordsValue.LittleAgeValue, compOfRecordsValue.OldAgeValue,
                  compOfRecordsValue.StartDayValue, compOfRecordsValue.EndDayValue,
                  compOfRecordsValue.PageStartRow, compOfRecordsValue.PageSize
                );

            List<MyCmpFaceLogWidthImgModel> listMyCmpFaceLogWidthImg = new List<MyCmpFaceLogWidthImgModel>();
            try
            {
                //比对结果
                for (int i = listCmpFaceLog.Count - 1; i >= 0; i--)
                {
                    MyCmpFaceLogWidthImgModel _MyCmpFaceLogWidthImg = new MyCmpFaceLogWidthImgModel();

                    //获得序号
                    if (listCmpFaceLog.Count == compOfRecordsValue.PageSize)//索引等于14
                    {
                        _MyCmpFaceLogWidthImg.num = compOfRecordsValue.MaxDataCount - compOfRecordsValue.PageStartRow - i;
                    }
                    else
                    {
                        if (compOfRecordsValue.MaxDataCount < compOfRecordsValue.PageSize)
                        {
                            _MyCmpFaceLogWidthImg.num = compOfRecordsValue.MaxDataCount - compOfRecordsValue.PageStartRow - i;
                        }
                        else
                        {
                            _MyCmpFaceLogWidthImg.num =
                               compOfRecordsValue.MaxDataCount - compOfRecordsValue.PageStartRow - i -
                               (compOfRecordsValue.PageSize - listCmpFaceLog.Count);
                        }
                    }

                    //获得通道名称 
                    foreach (MyChannelCfg mcc in QueryAllChannel())
                    {
                        if (listCmpFaceLog[i].Channel == mcc.TcChaneelID)
                        {
                            _MyCmpFaceLogWidthImg.channelName = mcc.Name;
                            _MyCmpFaceLogWidthImg.Address = mcc.Channel_address;
                            _MyCmpFaceLogWidthImg.Longitude = mcc.Longitude;
                            _MyCmpFaceLogWidthImg.Latitude = mcc.Latitude;
                        }
                    }

                    _MyCmpFaceLogWidthImg.ID = listCmpFaceLog[i].ID;// 标识ID
                    _MyCmpFaceLogWidthImg.channel = listCmpFaceLog[i].Channel;// 抓拍通道
                    long longTime = listCmpFaceLog[i].Time;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyCmpFaceLogWidthImg.date = time.ToString("yyyy/MM/dd");
                    _MyCmpFaceLogWidthImg.time = time.ToString("HH:mm:ss");// 抓拍时间
                    _MyCmpFaceLogWidthImg.SnapImageBuffer = listCmpFaceLog[i].Capimg;//

                    Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.SnapImage =
                    ImageConvert.ToBitmapImage(listCmpFaceLog[i].Capimg));//抓拍照片
                    Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.DBImage =
                    ImageConvert.ToBitmapImage(listCmpFaceLog[i].Senceimg));//场景照片

                    if (listCmpFaceLog[i].Ft.Count == 0)
                    {
                        _MyCmpFaceLogWidthImg.T1 = new CompOfRecordTemplate();
                        Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T1.TemplateImage =
                        ImageConvert.ToImageSource("pack://application:,,,/Images/unkonw.jpg"));
                    };

                    for (int j = 0; j < listCmpFaceLog[i].Ft.Count; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                _MyCmpFaceLogWidthImg.score = listCmpFaceLog[i].Ft[j].Score;
                                _MyCmpFaceLogWidthImg.type = TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                _MyCmpFaceLogWidthImg.T1 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T1.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T1.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T1.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T1.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            case 1:
                                _MyCmpFaceLogWidthImg.T2 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T2.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T2.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T2.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T2.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            case 2:
                                _MyCmpFaceLogWidthImg.T3 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T3.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T3.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T3.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T3.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            case 3:
                                _MyCmpFaceLogWidthImg.T4 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T4.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T4.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T4.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T4.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            case 4:
                                _MyCmpFaceLogWidthImg.T5 = new CompOfRecordTemplate();
                                _MyCmpFaceLogWidthImg.T5.UserName = "姓 名：" + listCmpFaceLog[i].Ft[j].Name;
                                _MyCmpFaceLogWidthImg.T5.TemplateType = "类 型：" + TemplateTypeConvert.GetTemplateType(listCmpFaceLog[i].Ft[j].Type);
                                Dispatcher.CurrentDispatcher.Invoke(() => _MyCmpFaceLogWidthImg.T5.TemplateImage =
                                ImageConvert.ToBitmapImage(listCmpFaceLog[i].Ft[j].Objimg));
                                _MyCmpFaceLogWidthImg.T5.LikeScore = "相似度：" + listCmpFaceLog[i].Ft[j].Score + "%";
                                break;
                            default:
                                break;
                        }
                    }

                    listMyCmpFaceLogWidthImg.Add(_MyCmpFaceLogWidthImg);
                }
            }
            catch (Exception ex)
            {
                Logger<ThirftService>.Log.Error("QueryCmpLog", ex);
            }
            return listMyCmpFaceLogWidthImg;
        }

        /// <summary>
        /// 查询比对记录
        /// </summary>
        /// <returns></returns>
        public List<MyCmpFaceLogWidthImgModel> QueryCmpLogOld(CompOfRecordsValue compOfRecordsValue)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            List<CmpFaceLog> listCmpFaceLog = SocketOpter.GetResult<ThirftService, List<CmpFaceLog>, string, string, int, int, int, int, long, long, int, int>(
                transport,
                bServerClient.QueryCmpLog,
                "QueryCmpLogOld ",
                compOfRecordsValue.ChannelValue, compOfRecordsValue.NameValue,
                   compOfRecordsValue.TypeValue, compOfRecordsValue.SexValue, compOfRecordsValue.LittleAgeValue, compOfRecordsValue.OldAgeValue, compOfRecordsValue.StartDayValue, compOfRecordsValue.EndDayValue, compOfRecordsValue.PageStartRow, compOfRecordsValue.PageSize
                );

            List<MyCmpFaceLogWidthImgModel> listMyCmpFaceLogWidthImg = new List<MyCmpFaceLogWidthImgModel>();
            try
            {
                //比对结果            
                for (int i = listCmpFaceLog.Count - 1; i >= 0; i--)
                {
                    MyCmpFaceLogWidthImgModel _MyCmpFaceLogWidthImg = new MyCmpFaceLogWidthImgModel();
                    //获得序号
                    _MyCmpFaceLogWidthImg.num = compOfRecordsValue.MaxDataCount - compOfRecordsValue.PageStartRow - i;
                    _MyCmpFaceLogWidthImg.ID = listCmpFaceLog[i].ID;// 标识ID
                    _MyCmpFaceLogWidthImg.name = listCmpFaceLog[i].Name;// 姓名

                    //获得通道名称 
                    foreach (MyChannelCfg mcc in QueryAllChannel())
                    {
                        if (listCmpFaceLog[i].Channel == mcc.TcChaneelID)
                        {
                            _MyCmpFaceLogWidthImg.channelName = mcc.Name;
                        }
                    }
                    //_MyCmpFaceLogWidthImg.channel = listCmpFaceLog[i].Channel;// 抓拍通道
                    long longTime = listCmpFaceLog[i].Time;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyCmpFaceLogWidthImg.time = time.ToString("yyyy/MM/dd HH:mm:ss");// 抓拍时间
                    foreach (var basicinfo in AppConfigs.DefFaceObjType)
                    {
                        if (basicinfo.Type == listCmpFaceLog[i].Type)
                        {
                            _MyCmpFaceLogWidthImg.type = basicinfo.Description;	// 类型  
                        }
                    }
                    _MyCmpFaceLogWidthImg.score = listCmpFaceLog[i].Score;// 相似度
                    _MyCmpFaceLogWidthImg.tcUuid = listCmpFaceLog[i].TcUuid; // uuid，模板ID

                    listMyCmpFaceLogWidthImg.Add(_MyCmpFaceLogWidthImg);
                }
            }
            catch (Exception ex)
            {
                Logger<ThirftService>.Log.Error("QueryCmpLogOld", ex);
            }
            return listMyCmpFaceLogWidthImg;
        }

        /// <summary>
        /// 查询比对(分区域)
        /// </summary>
        /// <param name="compOfRecordsValue"></param>
        /// <param name="pflag"></param>
        /// <returns></returns>
        public List<MyCmpFaceLogWidthImgModel> QueryCmpLogSXOld(CompOfRecordsValue compOfRecordsValue, int pflag)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            List<CmpFaceLog> listCmpFaceLog = SocketOpter.GetResult<ThirftService, List<CmpFaceLog>, string, string, int, int, int, int, long, long, int, int, int>(
                transport,
                bServerClient.QueryCmpLogSX,
                "QueryCmpLogSXOld ",
                compOfRecordsValue.ChannelValue, compOfRecordsValue.NameValue,
                   compOfRecordsValue.TypeValue, compOfRecordsValue.SexValue, compOfRecordsValue.LittleAgeValue, compOfRecordsValue.OldAgeValue, compOfRecordsValue.StartDayValue, compOfRecordsValue.EndDayValue, compOfRecordsValue.PageStartRow, compOfRecordsValue.PageSize, pflag
                );

            List<MyCmpFaceLogWidthImgModel> listMyCmpFaceLogWidthImg = new List<MyCmpFaceLogWidthImgModel>();
            try
            {
                //比对结果            
                for (int i = listCmpFaceLog.Count - 1; i >= 0; i--)
                {
                    MyCmpFaceLogWidthImgModel _MyCmpFaceLogWidthImg = new MyCmpFaceLogWidthImgModel();
                    //获得序号
                    _MyCmpFaceLogWidthImg.num = compOfRecordsValue.MaxDataCount - compOfRecordsValue.PageStartRow - i;
                    _MyCmpFaceLogWidthImg.ID = listCmpFaceLog[i].ID;// 标识ID
                    _MyCmpFaceLogWidthImg.name = listCmpFaceLog[i].Name;// 姓名

                    //获得通道名称
                    foreach (MyChannelCfg mcc in QueryAllChannel())
                    {
                        if (listCmpFaceLog[i].Channel == mcc.TcChaneelID)
                        {
                            _MyCmpFaceLogWidthImg.channelName = mcc.Name;
                        }
                    }
                    //_MyCmpFaceLogWidthImg.channel = listCmpFaceLog[i].Channel;// 抓拍通道
                    long longTime = listCmpFaceLog[i].Time;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyCmpFaceLogWidthImg.time = time.ToString("yyyy/MM/dd HH:mm:ss");// 抓拍时间
                    foreach (var basicinfo in AppConfigs.DefFaceObjType)
                    {
                        if (basicinfo.Type == listCmpFaceLog[i].Type)
                        {
                            _MyCmpFaceLogWidthImg.type = basicinfo.Description;	// 类型
                        }
                    }
                    _MyCmpFaceLogWidthImg.score = listCmpFaceLog[i].Score;// 相似度
                    _MyCmpFaceLogWidthImg.tcUuid = listCmpFaceLog[i].TcUuid; // uuid，模板ID

                    listMyCmpFaceLogWidthImg.Add(_MyCmpFaceLogWidthImg);
                }
            }
            catch (Exception ex)
            {
                Logger<ThirftService>.Log.Error("QueryCmpLogSXOld", ex);
            }
            return listMyCmpFaceLogWidthImg;
        }


        /// <summary>
        /// 查询比对记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<byte[]> QueryCmpLogImage(string ID)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<byte[]>, string>(
                transport,
                bServerClient.QueryCmpLogImage,
                "QueryCmpLogImage",
                ID
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<byte[]> QueryCmpLogImageH(string ID, string day)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<byte[]>, string, string>(
                transport,
                bServerClient.QueryCmpLogImageH,
                "QueryCmpLogImage",
                ID, day
                );
        }

        /// <summary>
        /// 查询模版照片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FaceObj> QueryFaceObj(string id)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<FaceObj>, string, string, int, int, int, int, long, long, int, int>(
                transport,
                bServerClient.QueryFaceObj,
                "QueryFaceObj 查询模版照片",
                id, null, -1, -1, -1, -1, -1, -1, -1, -1
                );
        }

        /// <summary>
        /// 抓拍记录数量
        /// </summary>
        /// <param name="captureRecordQueryValue"></param>
        /// <returns></returns>
        public int QueryCapRecordTotalCount(CaptureRecordQueryValue captureRecordQueryValue)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, int, string, long, long>(
                transport,
                bServerClient.QueryCapRecordTotalCount,
                "QueryCapRecordTotalCount 抓拍记录数量",
                captureRecordQueryValue.ChannelValue, captureRecordQueryValue.StartDayValue, captureRecordQueryValue.EndDayValue
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="captureRecordQueryValue"></param>
        /// <returns></returns>
        public List<SCountInfo> QueryCapRecordTotalCountH(CaptureRecordQueryValue captureRecordQueryValue)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<SCountInfo>, string, long, long>(
                transport,
                bServerClient.QueryCapRecordTotalCountH,
                "QueryCapRecordTotalCountH",
                captureRecordQueryValue.ChannelValue, captureRecordQueryValue.StartDayValue, captureRecordQueryValue.EndDayValue
                );
        }

        /// <summary>
        /// 查询比对记录数(筛选)
        /// </summary>
        /// <param name="captureRecordQueryValue"></param>
        /// <returns></returns>
        public List<SCountInfo> QueryCapRecordTotalCountHSXC(CaptureRecordQueryValue captureRecordQueryValue, List<string> channelName)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<SCountInfo>, List<string>, long, long>(
                transport,
                bServerClient.QueryCapRecordTotalCountHSXC,
                "QueryCapRecordTotalCountHSXC",
                channelName, captureRecordQueryValue.StartDayValue, captureRecordQueryValue.EndDayValue
                );
        }

        /// <summary>
        /// 抓拍记录数据
        /// </summary>
        /// <param name="captureRecordQueryValue"></param>
        /// <returns></returns>
        public List<MyCapFaceLogWithImg> QueryCapLog(CaptureRecordQueryValue captureRecordQueryValue)
        {
            List<CapFaceLog> listCapFaceLog = new List<CapFaceLog>();
            List<MyCapFaceLogWithImg> listMyCapFaceLogWithImg = new List<MyCapFaceLogWithImg>();
            //声明客户端内容
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            try
            {
                //获得查询数据 
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                listCapFaceLog = _BusinessServerClient.QueryCapLog(captureRecordQueryValue.ChannelValue, captureRecordQueryValue.StartDayValue, captureRecordQueryValue.EndDayValue, captureRecordQueryValue.StartRowValue, captureRecordQueryValue.PageRowValue);


                for (int i = listCapFaceLog.Count - 1; i >= 0; i--)
                {
                    MyCapFaceLogWithImg _MyCapFaceLogWithImg = new MyCapFaceLogWithImg();
                    _MyCapFaceLogWithImg.Id = captureRecordQueryValue.MaxCount - captureRecordQueryValue.StartRowValue - i;
                    _MyCapFaceLogWithImg.ID = listCapFaceLog[i].ID;// 获得抓拍id
                    _MyCapFaceLogWithImg.ChannelID = listCapFaceLog[i].ChannelID;// 获得通道id

                    //获得通道名称 
                    foreach (MyChannelCfg mcc in QueryAllChannel())
                    {
                        if (listCapFaceLog[i].ChannelID == mcc.TcChaneelID)
                        {
                            _MyCapFaceLogWithImg.ChannelName = mcc.Name;
                        }
                    }

                    long longTime = listCapFaceLog[i].Time;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyCapFaceLogWithImg.time = time.ToString("yyyy/MM/dd HH:mm:ss"); ;// 获得抓拍时间

                    listMyCapFaceLogWithImg.Add(_MyCapFaceLogWithImg);
                }
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试");
                Logger<ThirftService>.Log.Error("QueryCapLog", ex);
            }
            finally
            {
                transport.Close();
            }
            return listMyCapFaceLogWithImg;
        }

        /// <summary>
        /// 查询抓拍记录(筛选)
        /// </summary>
        /// <param name="captureRecordQueryValue"></param>
        /// <param name="pflag"></param>
        /// <returns></returns>
        public List<MyCapFaceLogWithImg> QueryCapLogSXC(CaptureRecordQueryValue captureRecordQueryValue, List<string> channelList)
        {
            List<CapFaceLog> listCapFaceLog = new List<CapFaceLog>();
            List<MyCapFaceLogWithImg> listMyCapFaceLogWithImg = new List<MyCapFaceLogWithImg>();
            //声明客户端内容
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            try
            {
                //获得查询数据 
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                listCapFaceLog = _BusinessServerClient.QueryCapLogSXC(channelList, captureRecordQueryValue.StartDayValue, captureRecordQueryValue.EndDayValue, captureRecordQueryValue.StartRowValue, captureRecordQueryValue.PageRowValue);


                for (int i = listCapFaceLog.Count - 1; i >= 0; i--)
                {
                    MyCapFaceLogWithImg _MyCapFaceLogWithImg = new MyCapFaceLogWithImg();
                    _MyCapFaceLogWithImg.Id = captureRecordQueryValue.MaxCount - captureRecordQueryValue.StartRowValue - i;
                    _MyCapFaceLogWithImg.ID = listCapFaceLog[i].ID;// 获得抓拍id
                    _MyCapFaceLogWithImg.ChannelID = listCapFaceLog[i].ChannelID;// 获得通道id

                    //获得通道名称 
                    foreach (MyChannelCfg mcc in QueryAllChannel())
                    {
                        if (listCapFaceLog[i].ChannelID == mcc.TcChaneelID)
                        {
                            _MyCapFaceLogWithImg.ChannelName = mcc.Name;
                        }
                    }

                    long longTime = listCapFaceLog[i].Time;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyCapFaceLogWithImg.time = time.ToString("yyyy/MM/dd HH:mm:ss"); ;// 获得抓拍时间

                    listMyCapFaceLogWithImg.Add(_MyCapFaceLogWithImg);
                }
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试");
                Logger<ThirftService>.Log.Error("QueryCapLogSXC", ex);
            }
            finally
            {
                transport.Close();
            }
            return listMyCapFaceLogWithImg;
        }

        /// <summary>
        /// 抓拍记录照片
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<byte[]> QueryCapLogImage(string ID)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            List<byte[]> listImageBytes = new List<byte[]>();
            try
            {
                #region
                //打开连接
                if (!transport.IsOpen)
                {
                    transport.Open();

                }
                //调用接口获得抓拍照片
                listImageBytes = _BusinessServerClient.QueryCapLogImage(ID);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("QueryCapLogImage", ex);
            }
            finally
            {
                transport.Close();
            }
            return listImageBytes;
        }

        public List<byte[]> QueryCapLogImageH(string ID, string day)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            List<byte[]> listImageBytes = new List<byte[]>();
            try
            {
                #region
                //打开连接
                if (!transport.IsOpen) transport.Open();
                //调用接口获得抓拍照片
                listImageBytes = _BusinessServerClient.QueryCapLogImageH(ID, day);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("QueryCapLogImageH", ex);
            }
            finally
            {
                if (transport.IsOpen) transport.Close();
            }
            return listImageBytes;
        }
        public List<byte[]> QuerySenceImg(string ID, string day)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            List<byte[]> listImageBytes = new List<byte[]>();
            try
            {
                #region
                //打开连接
                if (!transport.IsOpen)
                {
                    transport.Open();

                }
                //调用接口获得抓拍照片
                listImageBytes = _BusinessServerClient.QuerySenceImg(ID, day);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("QuerySenceImg", ex);
            }
            finally
            {
                transport.Close();
            }
            return listImageBytes;
        }

        /// <summary>
        /// 模板管理记录总数
        /// </summary>
        /// <param name="templateManagerValue"></param>
        /// <returns></returns>
        public int QueryFaceObjTotalCount(TemplateManagerViewModel.TemplateManagerValue template)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            int _nTemplatePageCounts = 0;
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }

                //获得查询总量
                _nTemplatePageCounts = _BusinessServerClient.QueryFaceObjTotalCount(null,
                    template.NameValue, template.LittleAgeValue, template.OldAgeValue, template.SexValue, template.TypeValue, template.StartDayValue, template.EndDayValue);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("QueryFaceObjTotalCount", ex);

            }
            finally
            {
                transport.Close();
            }
            return _nTemplatePageCounts;
        }

        /// <summary>
        /// 查询模版
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public List<MyFaceObj> QueryFaceObj(TemplateManagerViewModel.TemplateManagerValue template)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            List<FaceObj> _ListFaceObj = new List<FaceObj>();
            List<MyFaceObj> _ListMyFaceObj = new List<MyFaceObj>();
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                //获得查询数据
                _ListFaceObj = _BusinessServerClient.QueryFaceObj(null,
                    template.NameValue, template.LittleAgeValue, template.OldAgeValue, template.SexValue, template.TypeValue, template.StartDayValue, template.EndDayValue, template.StartRowValue, template.PageRowValue);

                List<string> sexList = new List<string>() { "未知", "男", "女" };

                for (int i = _ListFaceObj.Count - 1; i >= 0; i--)//循环得到人脸
                {
                    MyFaceObj _MyFaceObj = new MyFaceObj();
                    _MyFaceObj.faceObj = _ListFaceObj[i];
                    _MyFaceObj.ID = template.MaxCount - template.StartRowValue - i;
                    _MyFaceObj.fa_ob_tcUuid = _ListFaceObj[i].TcUuid;
                    _MyFaceObj.tcName = _ListFaceObj[i].TcName;// 姓名 
                    foreach (var basicinfo in AppConfigs.DefFaceObjType)
                    {
                        if (basicinfo.Type == _ListFaceObj[i].NType)
                        {
                            _MyFaceObj.nType = basicinfo.Description;	// 类型  
                        }
                    }

                    _MyFaceObj.nSex = sexList[_ListFaceObj[i].NSex];// 性别（0，未知；1，男；2，女）
                    _MyFaceObj.nMain_ftID = _ListFaceObj[i].NMain_ftID;
                    _MyFaceObj.nAge = _ListFaceObj[i].NAge;		// 年龄 

                    long longTime = _ListFaceObj[i].DTm;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyFaceObj.fa_ob_dTm = time.ToString();

                    _MyFaceObj.fa_ob_tcRemarks = _ListFaceObj[i].TcRemarks;

                    _ListMyFaceObj.Add(_MyFaceObj);
                }

                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("QueryFaceObjTotalCount", ex);
            }
            finally
            {
                transport.Close();
            }
            return _ListMyFaceObj;
        }

        /// <summary>
        /// 修改模版
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<ErrorInfo> ModifyFaceObj(string id, FaceObj obj)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol1 = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient1 = new BusinessServer.Client(protocol1);
            List<ErrorInfo> ListErrorInfo = new List<ErrorInfo>();
            try
            {
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                //调用修改接口，获得返回值
                ListErrorInfo = _BusinessServerClient1.ModifyFaceObj(id, obj);
                transport.Close();
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("QueryFaceObjTotalCount", ex);
            }
            finally
            {
                transport.Close();
            }
            return ListErrorInfo;
        }

        /// <summary>
        /// 删除模版
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int DelFaceObj(string ID)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            int nSucess = -1;
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                nSucess = _BusinessServerClient.DelFaceObj(ID);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("DelFaceObj", ex);
            }
            finally
            {
                transport.Close();
            }
            return nSucess;
        }

        /// <summary>
        /// 添加模版
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<ErrorInfo> AddFaceObj(FaceObj obj)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol1 = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient1 = new BusinessServer.Client(protocol1);
            List<ErrorInfo> ListErrorInfo = new List<ErrorInfo>();
            try
            {
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                //调用修改接口，获得返回值
                ListErrorInfo = _BusinessServerClient1.AddFaceObj(obj);
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
                Logger<ThirftService>.Log.Error("DelFaceObj", ex);
            }
            finally
            {
                transport.Close();
            }
            return ListErrorInfo;
        }

        /// <summary>
        /// 查询通道类型
        /// </summary>
        /// <returns></returns>
        public List<string> QueryDefChannelType()
        {
            List<string> listQueryDefChannelType = new List<string>();
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                listQueryDefChannelType = _BusinessServerClient.QueryDefChannelType();
                #endregion
            }
            catch (Exception ex)
            {
                Logger<ThirftService>.Log.Error("DelFaceObj", ex);
            }
            finally
            {
                transport.Close();
            }
            return listQueryDefChannelType;
        }

        /// <summary>
        /// 查询视频源类型
        /// </summary>
        /// <returns></returns>
        public List<string> QueryDefCameraType()
        {
            List<string> listQueryDefCameraType = new List<string>();
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                listQueryDefCameraType = _BusinessServerClient.QueryDefCameraType();
                #endregion
            }
            catch (Exception ex)
            {
            }
            finally
            {
                transport.Close();
            }
            return listQueryDefCameraType;
        }
        /// <summary>
        /// 修改频道
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public int ModifyChannel(ChannelCfgLBS cfg)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            int nSucess = -1;
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                nSucess = _BusinessServerClient.ModifyChannelLBS(cfg);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
            }
            finally
            {
                transport.Close();
            }
            return nSucess;
        }
        /// <summary>
        /// 添加频道
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public int AddChannel(ChannelCfgLBS cfg)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            int nSucess = -1;
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                nSucess = _BusinessServerClient.AddChannelLBS(cfg);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
            }
            finally
            {
                transport.Close();
            }
            return nSucess;
        }
        /// <summary>
        /// 添加频道
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public int AddChannel(ChannelCfg cfg)
        {
            TTransport transport = new TSocket(iHost, iPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            BusinessServer.Client _BusinessServerClient = new BusinessServer.Client(protocol);
            int nSucess = -1;
            try
            {
                #region
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                nSucess = _BusinessServerClient.AddChannel(cfg);
                #endregion
            }
            catch (Exception ex)
            {
                MB_MODULES.Views.MyMessage.showYes("网络异常，稍后重试！");
            }
            finally
            {
                transport.Close();
            }
            return nSucess;
        }

        /// <summary>
        /// 修改频道
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public int ModifyChannel(ChannelCfg cfg)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, int, ChannelCfg>(
                transport,
                bServerClient.ModifyChannel,
                "ModifyChannel",
                cfg
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageStrem"></param>
        /// <param name="threshold"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public List<MyFaceObj> QueryFaceObjByImg(byte[] imageStrem, int threshold, int maxCount)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            List<FaceObj> _ListFaceObj = SocketOpter.GetResult<ThirftService, List<FaceObj>, byte[], int, int>(
                transport,
                bServerClient.QueryFaceObjByImg,
                "QueryFaceObjByImg",
                imageStrem,
                threshold,
                maxCount
                );

            List<MyFaceObj> _ListMyFaceObj = new List<MyFaceObj>();
            try
            {
                List<string> sexList = new List<string>() { "未知", "男", "女" };
                for (int i = _ListFaceObj.Count - 1; i >= 0; i--)//循环得到人脸
                {
                    MyFaceObj _MyFaceObj = new MyFaceObj();
                    _MyFaceObj.faceObj = _ListFaceObj[i];
                    _MyFaceObj.ID = _ListFaceObj.Count - i;
                    _MyFaceObj.fa_ob_tcUuid = _ListFaceObj[i].TcUuid;
                    _MyFaceObj.tcName = _ListFaceObj[i].TcName;// 姓名 
                    foreach (var basicinfo in AppConfigs.DefFaceObjType)
                    {
                        if (basicinfo.Type == _ListFaceObj[i].NType)
                        {
                            _MyFaceObj.nType = basicinfo.Description;	// 类型  
                        }
                    }
                    _MyFaceObj.nSex = sexList[_ListFaceObj[i].NSex];// 性别（0，未知；1，男；2，女）
                    _MyFaceObj.nMain_ftID = _ListFaceObj[i].NMain_ftID;
                    _MyFaceObj.nAge = _ListFaceObj[i].NAge;		// 年龄 

                    long longTime = _ListFaceObj[i].DTm;
                    DateTime time = new DateTime(1970, 1, 1);
                    time = time.AddSeconds(longTime);
                    _MyFaceObj.fa_ob_dTm = time.ToString();

                    _MyFaceObj.fa_ob_tcRemarks = _ListFaceObj[i].TcRemarks;

                    _ListMyFaceObj.Insert(0, _MyFaceObj);
                }
            }
            catch (Exception ex)
            {
                Logger<ThirftService>.Log.Error("QueryFaceObjByImg", ex);
            }
            return _ListMyFaceObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<CmpFaceLogWidthImg> QueryCmpByCapIdWidthImg(string ID)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<CmpFaceLogWidthImg>, string>(
                 transport,
                 bServerClient.QueryCmpByCapIdWidthImg,
                 "QueryCmpByCapIdWidthImg",
                 ID
                 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<CmpFaceLogWidthImg> QueryCmpByCapIdWidthImgH(string ID, string day)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<CmpFaceLogWidthImg>, string, string>(
                transport,
                bServerClient.QueryCmpByCapIdWidthImgH,
                "",
                ID, day
                );
        }

        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public int DelChannel(string channelID)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, int, string>(
                transport,
                bServerClient.DelChannel,
                "DelChannel 删除通道",
                channelID
                );
        }

        /// <summary>
        /// 设置新的域值
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public int SetCMPthreshold(int threshold)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, int, int>(
                transport,
                bServerClient.SetCMPthreshold,
                "SetCMPthreshold 设置新的阈值",
                threshold
                );
        }

        /// <summary>
        /// 查询当前的域值
        /// </summary>
        /// <returns></returns>
        public int QueryThreshold()
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, int>(
                transport,
                bServerClient.QueryThreshold,
                "QueryThreshold 查询当前阈值"
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="uuid"></param>
        /// <param name="day"></param>
        /// <param name="pflag"></param>
        /// <returns></returns>
        public int UpdateCmpLog(string ID, string uuid, string day, int pflag)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, int, string, string, string, int>(
                transport,
                bServerClient.UpdateCmpLog,
                "UpdateCmpLog 更新比对记录",
                ID, uuid, day, pflag
                );
        }

        /// <summary>
        /// 查询性别定义
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<STypeInfo> QueryDefGenderH(int id)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<STypeInfo>, int>(
                transport,
                bServerClient.QueryDefGenderH,
                "QueryDefGenderH 查询性别",
                id
                );
        }

        /// <summary>
        /// 查询人脸对象类型定义
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<STypeInfo> QueryDefFaceObjTypeH(int id)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<STypeInfo>, int>(
                transport,
                bServerClient.QueryDefFaceObjTypeH,
                "QueryDefFaceObjTypeH 查询人脸对象类型定义",
                id
                );
        }

        /// <summary>
        /// 查询通道类型定义
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<STypeInfo> QueryDefChannelTypeH(int id)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<STypeInfo>, int>(
                transport,
                bServerClient.QueryDefChannelTypeH,
                "QueryDefChannelTypeH 查询通道类型定义",
                id
                );
        }

        /// <summary>
        /// 查询相机类型定义
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<STypeInfo> QueryDefCameraTypeH(int id)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<STypeInfo>, int>(
                transport,
                bServerClient.QueryDefChannelTypeH,
                "QueryDefCameraTypeH 查询相机类型定义",
                id
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capid"></param>
        /// <param name="capimg"></param>
        /// <param name="btime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public List<TrackInfo> QueryTrackPlayback(string capid, byte[] capimg, long btime, long etime)
        {
            BusinessServer.Client bServerClient = null;
            TTransport transport = SocketOpter.Init(iHost, iPort, 0, ref bServerClient);
            return SocketOpter.GetResult<ThirftService, List<TrackInfo>, string, byte[], long, long>(
                transport,
                bServerClient.QueryTrackPlayback,
                capid, capimg, btime, etime
                );
        }
    }
}
