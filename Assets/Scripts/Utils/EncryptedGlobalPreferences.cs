using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace CrazyGames.Logires.Utils
{
    public static class EncryptedGlobalPreferences
    {
        private static string _password = "null";

        static EncryptedGlobalPreferences()
        {
            Initialize("YI*HVU&Y4o4pOTFv^Q^70cw+MoSBnsk#lK|1waV2NvVf6J6voH+$XS3ymje8@jO_Xn79R+U|B#QtT-?C?b$&a_b0ZbL@N&ODd%RV|*YPovEWq$5z7kgncK%h$BCF%cPZqP_CKDMWMRuB#AMlkk0fLUMybygiFhL$Hv88DS$3%A5qBgTnJbZh@Nxn3sQ^*!pl7H2NbWk+Il%!ZsgThXO0y8W6TwOQG23%f9%pq+sECUtd+Yv3BOu$mwWsFrV*9PAqoze!y0?gyDj&esuuFXu||^f87h@$Bd-KGcTbl%i$bv6g9U=2Nkh##T+hwP@pD%Ozr%sVjoLLBD$V*lhuWipSe$B9zaJB_r?MweWfF@I7Z-FjNjB+_+S0denAXi+Y_ZkRVt#IYW*UPZ+rpCyA&c=CQuX3iDgVDcJ2oUw##SZmo&+Eimfyj9W=h802Ft_C=JEr-2SX7lUzi$6*y0^DdgclHpC^^u_yvqWqv9Bbpc7J?z_+UF6lzH15I$gS$HWsU$9R%6_At@y+mi+k=&@=lm^CuTBuDMJyoHwj%V^U&hP2S59^Z6Moen3AD#zGRF8spMT@1NgC-F6xuDIA5yYDY$XcafPxfb0WP_89PM#jiQA7HVDU67+6CDKrW1z|bjfT@P1yVJ&2+TzCUgq#Gl2rOe6IvDc_?sNuhiZJKm%M65%yQ@Ks2Jp?U&0t-?+-a+anSCPWkM4ff=X_pykMBdeGHTwGY1i2Y#iVjip|jyt5zCVJR*jRHTx@6H?zoOLy?iLZV^_i^HxXvdb_?R#_F-&0Gpv3-m8r!6M^18q9eETCE4q&|PvuaKfL_!RtSk#JaiDpG^0bc5ufJe?P-Wt#neOKe?%QdkL$8GblmXHCm4Qfm3zRT%YZazhRu#e7V%k0lO3QkhNi7$q&9ow*yoj^||S7FeWyW6D1c*zLtmp_M2XKlX?+7Ls?ZSDDM_rr$JL6GaPim4eqETx^WrCEMQjM#w7PgXeLOxZKtsFuP+0mSAY^sV#huSuej#I#6kIHfRNxFnJxIHF146ucqVH+TP!pdj9yCxq7IAaa4Qmhk93EJjv&AWy?c8cm$rBojDEml$2?1x=Q2thKp0T43=Q_*%e^&&8JA%#^ZiH_BRX-sr!BtSIj#lpyi4D_z41OBuUK^ZVZa&c#vnsw4JWWx6rogP*xLs*j06jHqKlTni0FmxSqCeG%o=Xef+hXoa3F-0#jQBsjoaifg-Lf%6?6TRLoF4WG9xI5lUcbNSyc@*siqZ+GuKyTQo=H!E65STEu!mW!ss**f_5TUJA!RqMag#@vLRk%1^HzPEjPK6Xah%y20Uc_7ZWoC0Ogca^TY51oBZ-Gd@@6IdBkT$9dz9aaQ@BVx4poivUu^Yo!$++aiem0FNnviWmHf&m$CEv7IXsMLEt4d&ktUqfGQAW*W=J0y49ozj0LGDZ6$@zDy4zib!D-%o%uR-9P|kzRRHT?P6d?8tKe=s$kjwpbI?=HJgJqaR@FP8P!!f|381x=YHw8E3IKRIoJuz2y_ErHng=ZwnoTL%o3GEw-xTZ-bm6_u77W3!l%tECFo-Bz?09xXn0&8?-IsrFT&Cn??7Cwz0zbm^B#@_?vTLks^*EmYWMmvgOu76lgWKK|=s^3KcOd6+VBzsK9wMqaGr&Ox+LoecIWCWNrd08P!pEN@t_AR0mBeUsfy0-SVO&nEz@^3RtYe35oIV@#qFt=x+|H!-^F5cHh?LG^fq1yTlxF1cKAKDXvKWam1xf|5OtaHuVCD@vWW*+B4N%uuJebWtB7hr|3u$8ds_d|vkImWS3JzybuFlRBfirv0ETkyd$kYyR&uR0t_r813hmD8GJy2Oro0tpA01agP|*ZNxj^QHg+SCB6O5FZMt__5^OxvfhX$l3UVc2TBnvjmqBp01wK1Y#A2Ko16h|OlhifkBLSb9#B=@nGg*VSkE5$-zkQh|d+kCBnhfaWhM1Huy4VPB%YrC2=+=1s-m!MU-lCd%BWIKlAD|idC1#&$wKb%qA4Nb6mET@t=R?NR$!Y7Hx1XruUSjDL#ad@zvd5#S0v7JyyzQ|R&$wzYMd5");
        }

        public static void Initialize(string password = "")
        {
            if (string.IsNullOrEmpty(password))
            {
                password = "null";
            }

            _password = password;
        }

        public static void Set(string key, object value)
        {
            var encrypted = AesEncryptor.Encrypt(_password, JsonConvert.SerializeObject(value));

            GlobalPreferences.Set(key, encrypted);
        }

        public static object Get(string key)
        {
            return Get<object>(key, null);
        }

        public static T Get<T>(string key) where T : class
        {
            var data = GlobalPreferences.Get(key, "");

            var decrypted = AesEncryptor.Decrypt(_password, data);

            return JsonConvert.DeserializeObject<T>(decrypted);
        }

        public static T Get<T>(string key, T defaultValue) where T : class
        {
            var result = Get<T>(key);

            if (result != null)
            {
                return result;
            }

            return defaultValue;
        }
    }
}