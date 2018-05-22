using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace ConnectLab
{
    public class PrintKetQuaNuocTieu
    {
        public string MaPhieuCLS = "PT-141000936CT";
        public string LoaiMay = "Urisys";
        private DataTable loadBN()
        {
            if (MaPhieuCLS == null || this.MaPhieuCLS == "") return null;
            string sqlBN = @"
                SELECT DISTINCT DienThoaiCN=CN.DienThoai
                ,DiaChiCN=cn.DiaChi
                ,EmailCN=CN.Email
                ,TenBenhNhan=bn.TenBenhNhan
                ,NamSinh=dbo.GetNamSinh(bn.ngaysinh)
                ,GioiTinh=dbo.GetGioiTinh(bn.gioitinh)
                ,TenDonVi=CTY.tencty
                ,MaPhieuCLS=cls.MaPhieuCLS+ ' - '+ bn.mabenhnhan
                ,DienThoai=bn.dienthoai
                ,TenBsChiDinh=case when kb.IsKhamSucKhoe=1 then 
					                 case when CN.Ten_Cty IS NULL then N'Trung tâm y tế Tân Cảng' else CN.Ten_Cty end
				                else
					                 BS.TENBACSI 
				                end
                ,ChanDoan= case when kb.IsKhamSucKhoe=1 then N'Khám sức khỏe định kỳ'
				                else kb.MoTaCD_edit end
                ,NgayThangNam=N'Ngày '+ CONVERT(varchar,DAY(CLS.ngaythu))+ N' tháng '+CONVERT(varchar, month(CLS.ngaythu)) + N' năm '+ CONVERT(varchar,year(CLS.ngaythu))
                FROM khambenhcanlamsan CLS INNER JOIN BENHNHAN BN ON BN.idbenhnhan=CLS.idbenhnhan
                left join khambenh kb on kb.idkhambenh =CLS.idkhambenh
                LEFT JOIN BACSI BS ON KB.IDBACSI=BS.IDBACSI
                LEFT JOIN KB_CongTy CTY ON CTY.idcongty= BN.idcongty
                LEFT JOIN TieuDeCty CN ON CN.IdChiNhanh= ISNULL((SELECT TOP 1 IDCHINHANH FROM DANGKYKHAM WHERE IDBENHNHAN =BN.IDBENHNHAN ORDER BY iddangkykham DESC),1)
                WHERE CLS.MaPhieuCLS='" + MaPhieuCLS + "' ";

            DataTable dtBN = (IsTestOne != true ? DataAcess.Connect.GetTable(sqlBN) : DataAcess.Connect.GetTable(sqlBN));

            return dtBN;
        }
       public bool IsTestOne = true;
        private DataTable loadDichVu()
        {
            #region sql Select

            if (MaPhieuCLS == null || this.MaPhieuCLS == "") return null;
            string sqlDV = @"SELECT maketqua=CLS.MaPhieuCLS
                                    , BatThuong=CONVERT(BIT,0)
                                    ,DV.IDBANGGIADICHVU
                                    ,DV.TENNHOM,DV.TENDICHVU
                                    ,loaimay= N'NƯỚC TIỂU' 
                                    ,KETQUA=ISNULL( (
                                                                          SELECT TOP 1    KQCT.giatrichuoi 
                                                                            FROM KetQuaTuMayCLS_CT KQCT 
                                                                            INNER JOIN KetQuaTuMayCLS KQ ON KQ.idKetQuaTuMayCLS = KQCT.idKetQuaTuMayCLS
                                                                            WHERE KQ.maketqua='" + MaPhieuCLS + @"'
                                                                                        AND KQCT.tenthongso=CT.MACHITIET
                                                                                        AND KQ.LOAIMAY='Urisys'    
                                                                                    ORDER BY KQCT.idKetQuaTuMayCLS DESC
                                                    )
                                                      ,CONVERT(NVARCHAR(20)
                                                    ,(
                                                                          SELECT TOP 1  KQCT.GIATRI
                                                                            FROM KetQuaTuMayCLS_CT KQCT 
                                                                            INNER JOIN KetQuaTuMayCLS KQ ON KQ.idKetQuaTuMayCLS = KQCT.idKetQuaTuMayCLS
                                                                            WHERE KQ.maketqua='" + MaPhieuCLS + @"'
                                                                                AND KQCT.tenthongso=CT.MACHITIET
                                                                                AND KQ.LOAIMAY='Urisys'
                                                                                    ORDER BY KQCT.idKetQuaTuMayCLS DESC
                                                       )
                                                    ))
                                     ,GIATRIBT=CT.giatribinhthuong
                                    ,ct.DONVI
                                    ,TENCHITIET= case when ct.InDam =1 then ct.TENCHITIET else N'    '+ ct.TENCHITIET end
                                     ,ct.tennhom NhomCT
                                    , SLCT=  0    
                                    ,InDam=ct.indam
                                                        FROM
                                                             KHAMBENHCANLAMSAN CLS
                                                            INNER join banggiadichvu dv ON CLS.IDCANLAMSAN=DV.IDBANGGIADICHVU
                                                            left join chitietdichvu ct on ct.idbanggiadichvu=dv.idbanggiadichvu
                                                            where CLS.MaPhieuCLS='" + MaPhieuCLS + @"' 
                                                            AND DV.idphongkhambenh=22
                                                            and dv.TENNHOM LIKE  N'%Nước tiểu%' 
                                                        order by dv.tennhom,dv.stt,ct.stt
                                                        
                                    ";
            DataTable dtDV = (IsTestOne != true ? DataAcess.Connect.GetTable(sqlDV) : DataAcess.Connect.GetTable(sqlDV));
            if (dtDV == null) return null;
            if (dtDV.Rows.Count == 0) return dtDV;


            #endregion
            #region Xét giới tính nam nữ
            bool IsXetNamNu = true;
            int nNamNuTemp = Suport.hsTool.int_Search(dtDV, "GIATRIBT LIKE '%Nữ%'");
            if (nNamNuTemp != -1) IsXetNamNu = true;
            bool IsGioiTinhNam = false;
            if (IsXetNamNu)
            {
                string sqlSelectGioiTinh = @"SELECT TOP 1  B.GIOITINH FROM KHAMBENHCANLAMSAN A INNER JOIN BENHNHAN B ON A.IDBENHNHAN=B.IDBENHNHAN  WHERE A.MAPHIEUCLS='" + this.MaPhieuCLS + "'";
                DataTable dtGetGioiTinh = (IsTestOne != true ? DataAcess.Connect.GetTable(sqlSelectGioiTinh) : DataAcess.Connect.GetTable(sqlSelectGioiTinh));
                if (dtGetGioiTinh != null && dtGetGioiTinh.Rows.Count > 0)
                {
                    IsGioiTinhNam = !Suport.hsTool.getCheck(dtGetGioiTinh.Rows[0][0].ToString());
                }
            }
            #endregion
            #region chỉ lấy kết quả cần thiết
            if (dtDV != null && dtDV.Rows.Count > 0)
            {
                dtDV.DefaultView.RowFilter = "loaimay<>'' OR KETQUA<>''";
                dtDV = dtDV.DefaultView.ToTable();
            }
            #endregion
            #region Xử lý từng kết quả
            for (int i = 0; i < dtDV.Rows.Count; i++)
            {
                try
                {
                    string iKetQua = dtDV.Rows[i]["KETQUA"].ToString();
                    if (iKetQua != "")
                    {
                        #region Xử lý kí tự .
                        string iiKetQua = "";
                        for (int j = 0; j < iKetQua.Length; j++)
                            if (char.IsNumber(iKetQua[j]) == true || iKetQua[j].ToString() == ".")
                                iiKetQua = iiKetQua + iKetQua[j].ToString();
                        #endregion
                        #region Xử lý In đậm
                        try
                        {
                            string GIATRIBT = dtDV.Rows[i]["GIATRIBT"].ToString();
                            if (IsXetNamNu && GIATRIBT.ToLower().IndexOf("Nữ".ToLower()) != -1)
                            {
                                try
                                {
                                    int nTemp = GIATRIBT.ToLower().IndexOf("Nữ".ToLower());
                                    string GIATRIBT_NAM = GIATRIBT.Substring(0, nTemp);
                                    string GIATRIBT_Nu = GIATRIBT.Substring(nTemp, GIATRIBT.Length - nTemp);
                                    GIATRIBT_NAM = GIATRIBT_NAM.Replace("Nam", "");
                                    GIATRIBT_NAM = GIATRIBT_NAM.Replace("nam", "");
                                    GIATRIBT_NAM = GIATRIBT_NAM.Replace("(", "");
                                    GIATRIBT_NAM = GIATRIBT_NAM.Replace(")", "");
                                    GIATRIBT_NAM = GIATRIBT_NAM.Replace(";", "");

                                    GIATRIBT_Nu = GIATRIBT_Nu.Replace("Nữ", "");
                                    GIATRIBT_Nu = GIATRIBT_Nu.Replace("nữ", "");
                                    GIATRIBT_Nu = GIATRIBT_Nu.Replace("(", "");
                                    GIATRIBT_Nu = GIATRIBT_Nu.Replace(")", "");
                                    GIATRIBT_Nu = GIATRIBT_Nu.Replace(";", "");

                                    GIATRIBT = GIATRIBT_NAM;
                                    if (IsGioiTinhNam == false) GIATRIBT = GIATRIBT_Nu;
                                }
                                catch (Exception) { }
                            }

                            double dKetQua = double.Parse(iiKetQua);
                            string[] arrGiaTriBinhThuong = GIATRIBT.Split('-');
                            if (arrGiaTriBinhThuong.Length == 2)
                            {
                                double dCanTren = double.Parse(arrGiaTriBinhThuong[0]);
                                double dCanDuoi = double.Parse(arrGiaTriBinhThuong[1]);
                                if (dKetQua < dCanTren || dKetQua > dCanDuoi)
                                {
                                    dtDV.Rows[i]["BatThuong"] = true;
                                    if (dKetQua < dCanTren)
                                        dtDV.Rows[i]["SLCT"] = 1;
                                    else
                                        dtDV.Rows[i]["SLCT"] = 2;

                                }

                            }
                            else
                                if (GIATRIBT.IndexOf(">") != -1 || GIATRIBT.IndexOf("<") != -1)
                                {
                                    if (GIATRIBT.IndexOf(">") != -1)
                                    {
                                        string[] arrGiaTriBinhThuong1 = GIATRIBT.Split('>');
                                        if (dKetQua <= double.Parse(arrGiaTriBinhThuong1[1]))
                                        {
                                            dtDV.Rows[i]["BatThuong"] = true;
                                            dtDV.Rows[i]["SLCT"] = 1;
                                        }
                                    }

                                    if (GIATRIBT.IndexOf("<") != -1)
                                    {
                                        string[] arrGiaTriBinhThuong2 = GIATRIBT.Split('<');
                                        if (dKetQua >= double.Parse(arrGiaTriBinhThuong2[1]))
                                        {
                                            dtDV.Rows[i]["BatThuong"] = true;
                                            dtDV.Rows[i]["SLCT"] = 2;
                                        }
                                    }


                                }
                                else
                                    if (dtDV.Rows[i]["TENCHITIET"].ToString() == "RBC" || dtDV.Rows[i]["TENCHITIET"].ToString() == "PLT" || dtDV.Rows[i]["TENCHITIET"].ToString() == "WBC")
                                        dtDV.Rows[i]["BatThuong"] = true;
                                    else


                                        dtDV.Rows[i]["BatThuong"] = false;

                            dtDV.Rows[i]["KETQUA"] = iiKetQua;

                        }
                        catch (Exception)
                        {

                        }
                        #endregion

                    }
                }
                catch (Exception) { }

            }
            #endregion
            return dtDV;
        }
        

        public void PrintKetQua()
        {
            DataTable dtBN = loadBN();
            DataTable dtDV = loadDichVu();
            if (dtBN == null) return;
            if (dtDV == null) return;
            try
            {
                rptKQXetNghiemTieuDe R = new rptKQXetNghiemTieuDe();
                dtDV.TableName = "dtXetNghiem";
                R.SetDataSource(dtDV);

                for (int i = 0; i < dtBN.Columns.Count; i++)
                {
                    try
                    {
                        R.SetParameterValue(dtBN.Columns[i].ToString(), dtBN.Rows[0][i].ToString());
                    }
                    catch (Exception) { }
                }
                
                    R.PrintToPrinter(0, true, 1, 100);
                    R.Dispose();
                    try
                    {
                        string sqlSaveKetQuaCanLamSan = @" EXEC  zHS_CreateKetQuaCanLamSan '" + this.MaPhieuCLS + "'";
                        DataAcess.Connect.ExecSQL(sqlSaveKetQuaCanLamSan);
                    }
                    catch (Exception) { }
                
            }
            catch (Exception E) { }

        }
       public static int int_Search(DataTable dtSearch, string s_Filter)
       {
           try
           {
               DataRow[] DR = dtSearch.Select(s_Filter);
               if (DR.Length == 0) return -1;
               return dtSearch.Rows.IndexOf(DR[0]);
           }
           catch (Exception)
           {
               return -1;
           }
       }
    }
}
