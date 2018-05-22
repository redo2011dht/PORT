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
   public class PrintKetQuaXN
    {
       public string MaPhieuCLS = "000";
       public string idKetQuaTuMayCLS = "0000";
        public string LoaiMay = "";
        private DataTable loadBN()
        {
            if (MaPhieuCLS == null || this.MaPhieuCLS == "") return null;
            string sqlBN = @"
                SELECT DISTINCT DienThoaiCN=bn.mabenhnhan
                ,DiaChiCN=cn.DiaChi
                ,EmailCN=CN.Email
                ,TenBenhNhan=bn.TenBenhNhan
                ,NamSinh=dbo.GetNamSinh(bn.ngaysinh)
                ,GioiTinh=dbo.GetGioiTinh(bn.gioitinh)
                ,TenDonVi=CTY.tencty
                ,MaPhieuCLS=cls.MaPhieuCLS
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
           // if (MaPhieuCLS != "PT-151000765CT") return null;
            #region sql Select
            #region Check 
            if (idKetQuaTuMayCLS != null && idKetQuaTuMayCLS != "" && idKetQuaTuMayCLS != "0000")
            {

                string sqlCheckAgain = @"SELECT TOP 1 1 FROM KetQuaTuMayCLS WHERE IDKETQUATUMAYCLS='" + idKetQuaTuMayCLS + @"' AND ISNULL(ISPRINT,0)=1";
                DataTable dtCheckAgain = DataAcess.Connect.GetTable(sqlCheckAgain);
                if (dtCheckAgain == null) return null;
               // if (dtCheckAgain.Rows.Count > 0) return null;

                string sqlDetailKetQua_check = @"SELECT idKetQuaTuMayCLS,ketqua=ISNULL(giatrichuoi ,CONVERT(NVARCHAR(20),GIATRI) )
                                                        FROM KETQUATUMAYCLS_CT WHERE IDKETQUATUMAYCLS=" + idKetQuaTuMayCLS + @"
                                                            AND ISNULL(giatrichuoi ,CONVERT(NVARCHAR(20),GIATRI) )<>''
                                                             AND ISNULL(giatrichuoi ,CONVERT(NVARCHAR(20),GIATRI) )<>'0'
                                                ";


                DataTable dtDetailCheck = DataAcess.Connect.GetTable(sqlDetailKetQua_check);
                if (dtDetailCheck == null) return null;
                if (dtDetailCheck.Rows.Count == 0) return null;


            }
            
            #endregion

            if (MaPhieuCLS == null || this.MaPhieuCLS == "") return null;
            DataTable dtDV = null;
           
                string sqlSelect_New = @" SELECT * FROM DBO.ZHS_KETQUAXETNGHIEM ('" + this.MaPhieuCLS + "')  order by tennhom  ,STT_TB ASC , NhomCT ASC ,STT_CT ASC ,KETQUA DESC ";
                dtDV = (IsTestOne != true ? DataAcess.Connect.GetTable(sqlSelect_New) : DataAcess.Connect.GetTable(sqlSelect_New));
                if (dtDV == null) return null;
                dtDV.DefaultView.RowFilter = "loaimay='Au480' or loaimay='LH500'";
                dtDV = dtDV.DefaultView.ToTable();
                List<string> lst = new List<string>();
                dtDV.Columns.Add("NoView1");
                for (int i = 0; i < dtDV.Rows.Count; i++)
                {
                    string svalue = dtDV.Rows[i]["idbanggiadichvu"].ToString() + ";" + dtDV.Rows[i]["tenchitiet"].ToString();
                    if (lst.IndexOf(svalue) == -1)
                    {
                        if (dtDV.Rows[i]["loaimay"].ToString() == "Au480" || dtDV.Rows[i]["loaimay"].ToString() == "LH500")
                        {
                            string MaChiTiet = dtDV.Rows[i]["MaChiTiet"].ToString();
                            if (dtDV.Rows[i]["loaimay"].ToString() == "Au480")
                            {
                                if (Array.IndexOf(Au480Connect.arrSH, MaChiTiet) == -1)
                                    dtDV.Rows[i]["NoView1"] = "1";
                                else
                                {
                                    lst.Add(svalue);
                                    dtDV.Rows[i]["NoView1"] = "0";
                                }
                            }
                            else
                            {
                                if (Array.IndexOf(Lh500Connect.arrHH, MaChiTiet) == -1)
                                    dtDV.Rows[i]["NoView1"] = "1";
                                else
                                {
                                    lst.Add(svalue);
                                    dtDV.Rows[i]["NoView1"] = "0";
                                }
                            }

                        }
                        else
                        {
                            lst.Add(svalue);
                            dtDV.Rows[i]["NoView1"] = "0";
                        }
                    }
                    else
                    {
                        
                            dtDV.Rows[i]["NoView1"] = "1";
                    }
                }
                int count1 = dtDV.Rows.Count;
                dtDV.DefaultView.RowFilter="NoView1='0'";
                dtDV=dtDV.DefaultView.ToTable();
                int count2 = dtDV.Rows.Count;
                if (count1 > count2)
                {
                }
           
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
            #region Kiểm tra kết quả khác



            int nSinnhhoa = int_Search(dtDV, "loaimay='Au480'");
            if (nSinnhhoa != -1)
            {
                DataView dv = new DataView(dtDV);
                dv.RowFilter = "loaimay='Au480'";
                int n_totalSinhHoa = dv.Count;

                dv.RowFilter = "loaimay='Au480' AND KETQUA<>''";
                int n_totalSinhHoa_KoCoKetqua = dv.Count;

                if (n_totalSinhHoa != n_totalSinhHoa_KoCoKetqua)
                {
                    return null;
                }

            }


            int nHuyetHoc = int_Search(dtDV, "loaimay='LH500'");
            if (nHuyetHoc != -1)
            {
                DataView dv2 = new DataView(dtDV);
                dv2.RowFilter = "loaimay='LH500'";
                int n_totalLH500 = dv2.Count;

                dv2.RowFilter = "loaimay='LH500' AND KETQUA<>''";
                int n_totalLH500_KoCoKetqua = dv2.Count;

                if (n_totalLH500 != n_totalLH500_KoCoKetqua)
                {
                    return null;

                }
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
            if (dtBN.Rows.Count == 0) return;
            try
            {
                ConnectLab.rptKQXetNghiemTieuDe R = new ConnectLab.rptKQXetNghiemTieuDe();
                dtDV.TableName = "dtXetNghiem";
                R.SetDataSource(dtDV);
 
                    try
                    {
                        R.SetParameterValue("DienThoaiCN",dtBN.Rows[0]["DienThoaiCN"].ToString());
                        R.SetParameterValue("DiaChiCN",dtBN.Rows[0]["DiaChiCN"].ToString());
                        R.SetParameterValue("TenBenhNhan",dtBN.Rows[0]["TenBenhNhan"].ToString());
                        R.SetParameterValue("NamSinh",dtBN.Rows[0]["NamSinh"].ToString());
                        R.SetParameterValue("GioiTinh",dtBN.Rows[0]["GioiTinh"].ToString());
                        R.SetParameterValue("TenDonVi",dtBN.Rows[0]["TenDonVi"].ToString());
                        R.SetParameterValue("MaPhieuCLS",dtBN.Rows[0]["MaPhieuCLS"].ToString());
                        R.SetParameterValue("DienThoai",dtBN.Rows[0]["DienThoai"].ToString());
                        R.SetParameterValue("TenBsChiDinh",dtBN.Rows[0]["TenBsChiDinh"].ToString());
                        R.SetParameterValue("ChanDoan",dtBN.Rows[0]["ChanDoan"].ToString());
                        R.SetParameterValue("NgayThangNam", dtBN.Rows[0]["NgayThangNam"].ToString());
                    }
                    catch (Exception) { }
                 
                try
                {
                    DataTable  dtTieuDeCty = DataAcess.Connect.GetTable("SELECT * FROM TIEUDECTY ");
                    ((CrystalDecisions.CrystalReports.Engine.TextObject)R.ReportDefinition.ReportObjects["tencty0"]).Text = dtTieuDeCty.Rows[0]["tengoc"].ToString();
                    ((CrystalDecisions.CrystalReports.Engine.TextObject)R.ReportDefinition.ReportObjects["tencty1"]).Text = dtTieuDeCty.Rows[0]["Ten_Cty"].ToString();
                    ((CrystalDecisions.CrystalReports.Engine.TextObject)R.ReportDefinition.ReportObjects["diachicty"]).Text = dtTieuDeCty.Rows[0]["diachi"].ToString();
                    ((CrystalDecisions.CrystalReports.Engine.TextObject)R.ReportDefinition.ReportObjects["dienthoaicty"]).Text = dtTieuDeCty.Rows[0]["dienthoai"].ToString();
                }
                catch (Exception) { }

                if (1 == 1)
                    R.PrintToPrinter(0, true, 1, 100);
                else MessageBox.Show(this.MaPhieuCLS);

                    R.Dispose();
                    try
                    {
                        string sqlSaveKetQuaCanLamSan = @" 
                                                            UPDATE KetQuaTuMayCLS SET ISPRINT=1 WHERE maketqua='" + this.MaPhieuCLS + @"'
                                                            EXEC  DBO.zHS_CreateKetQuaCanLamSan '" + this.MaPhieuCLS + "'";
                        bool OK =   DataAcess.Connect.ExecSQL(sqlSaveKetQuaCanLamSan);
                      if (!OK)
                      {
                          MessageBox.Show("Không thể In được, vui lòng Tắt phần mềm, mở lại thử xem");
                          return;
                      }
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
