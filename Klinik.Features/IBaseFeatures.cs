namespace Klinik.Features
{
    /// <summary>
    /// The interface of base feature
    /// </summary>
    /// <typeparam name="TResp"></typeparam>
    /// <typeparam name="TReq"></typeparam>
    public interface IBaseFeatures<TResp, TReq> where TResp : class where TReq : class
    {
        TResp GetListData(TReq request);
        TResp CreateOrEdit(TReq request);
        TResp GetDetail(TReq request);
        TResp RemoveData(TReq request);
    }
}