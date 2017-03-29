using FaceSysClient.ClassPool;
using System.Collections.Generic;
using static FaceSysByMvvm.ViewModels.CaptureRecordQuery.CaptureRecordQueryViewModel;
using static FaceSysByMvvm.ViewModels.CompOfRecords.CompOfRecordsViewModel;
using static FaceSysByMvvm.ViewModels.TemplateManager.TemplateManagerViewModel;
using FaceSysByMvvm.Model;

namespace ThriftService
{
    public interface IThirtfService
    {
        /// <summary>
        /// 登录函数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        bool Login(string ip, int port);

        //心跳
        int HearBeat();

        /// <summary>
        /// 获取所有频道
        /// </summary>
        /// <returns></returns>
        List<MyChannelCfg> QueryAllChannel();

        /// <summary>
        /// 获取模版类型
        /// </summary>
        /// <returns></returns>
        List<string> QueryDefFaceObjType();

        /// <summary>
        /// 获取模版数量
        /// </summary>
        /// <returns></returns>
        int QueryCmpRecordTotalCount(CompOfRecordsValue compOfRecordsValue);
        
        /// <summary>
        /// 获取模板记录
        /// </summary>
        /// <param name="compOfRecordsValu">ViewModel</param>
        /// <returns></returns>
        List<MyCmpFaceLogWidthImgModel> QueryCmpLogOld(CompOfRecordsValue compOfRecordsValu);

        //查询抓拍的照片
        List<byte[]> QueryCmpLogImage(string ID);

        //查询对比照片
        List<FaceObj> QueryFaceObj(string id);

        
        //抓拍记录总数
        int QueryCapRecordTotalCount(CaptureRecordQueryValue captureRecordQueryValue);

        //抓拍记录数据
        List<MyCapFaceLogWithImg> QueryCapLog(CaptureRecordQueryValue captureRecordQueryValue);
        //抓拍记录照片
        List<byte[]> QueryCapLogImage(string ID);


        //模板管理记录总数
        int QueryFaceObjTotalCount(TemplateManagerValue templateManagerValue);

        //模版管理记录信息
        List<MyFaceObj> QueryFaceObj(TemplateManagerValue templateManagerValue);

        //修改模版
        List<ErrorInfo> ModifyFaceObj(string id, FaceObj obj);

        //删除模版
        int DelFaceObj(string ID);

        //增加模版
        List<ErrorInfo> AddFaceObj(FaceObj obj);
        //通道类型
        List<string> QueryDefChannelType();
        //视频源类型
        List<string> QueryDefCameraType();
        //添加频道
        int AddChannel(ChannelCfg cfg);
        //修改频道
        int ModifyChannel(ChannelCfg cfg);

        //图片查询
        List<MyFaceObj> QueryFaceObjByImg(byte[] image, int nThreshold, int nMaxCount);

        //根据抓拍ID获取全部比对照片
        List<CmpFaceLogWidthImg> QueryCmpByCapIdWidthImg(string ID);

        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="channelID"></param>
        /// <returns></returns>
        int DelChannel(string channelID);

        /// <summary>
        /// 设置比对的域值
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        int SetCMPthreshold(int threshold);






    }
}
