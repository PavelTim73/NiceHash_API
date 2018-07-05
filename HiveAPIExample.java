package org.hive.api;

import org.json.JSONException;
import org.json.JSONObject;

import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;
import javax.net.ssl.HttpsURLConnection;
import javax.xml.bind.DatatypeConverter;
import java.io.*;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;
import java.net.URLEncoder;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.util.HashMap;
import java.util.Map;

public class HiveAPIExample {
    private static final String UTF_8 = "UTF-8";
    private static final String GET_RIGS = "getRigs";
    private static final String GET_WALLETS = "getWallets";
    private static final String GET_OC = "getOC";
    private static final String GET_CURRENT_STATS = "getCurrentStats";
    private static final String MULTI_ROCKET = "multiRocket";

    /**
     * Your secret API key, you sign the message with it
     * @var string 64 hex chars
     */
    private static String SECRET_KEY = "xxx";

    /**
     * This is a public key
     * @var string 64 hex chars
     */
    private static String PUBLIC_KEY = "xxx";

    /**
     * Api connection URL
     * Please use HTTPS, not HTTP for calls for better security.
     * @var string
     */
    public static String HTTPS_URL = "https://api.hiveos.farm/worker/eypiay.php";

    static class Response{
        int responseCode = -1;
        JSONObject responseData = null;
    }


    static class MultiRocketParams{
        String rigIds;         // coma separated string with rig ids "1,2,3,4"
        String miner = null;   // Miner to set. Leave it null if you do not want to change. "claymore", "claymore-z", "ewbf", ...
        String miner2 = null;  // Second miner to set. Leave it null if you do not want to change. "0" - if you want to unset it.
        String idWal = null;   // ID of wallet. Leave it null if you do not want to change.
        String idOc = null;    // ID of OC profile. Leave it null if you do not want to change.
    }

    private static void log(String msg) {
        System.out.println(msg);
    }

    private final String[] args;

    private HiveAPIExample(String[] args){
        this.args = args;
    }

    private static void printHelp(){
        System.out.println("Usage: java -jar HiveAPIExample.jar methodName aguments");
        System.out.println("Examples:");
        System.out.println("\tjava -jar HiveAPIExample.jar " + GET_RIGS);
        System.out.println("\tjava -jar HiveAPIExample.jar " + GET_WALLETS);
        System.out.println("\tjava -jar HiveAPIExample.jar " + GET_CURRENT_STATS);
        System.out.println("\tjava -jar HiveAPIExample.jar " + MULTI_ROCKET + " \"rig1,rig2\" [miner] [miner2] [id_wal] [id_oc]");
    }

    public static void main(String[] args) throws Exception {
        if(args.length >= 1){
            HiveAPIExample h = new HiveAPIExample(args);
            h.run();
        }else{
            printHelp();
        }
    }

    private void run() throws Exception {
        log("=== Hive API example ===");

        String method = args[0];
        JSONObject result = null;

        if(GET_RIGS.equalsIgnoreCase(method)){
            result = getRigs();
        } else if(GET_WALLETS.equalsIgnoreCase(method)){
            result = getWallets();
        } else if(GET_OC.equalsIgnoreCase(method)){
            result = getOC();
        } else if(GET_CURRENT_STATS.equalsIgnoreCase(method)){
            result = getCurrentStats();
        } else if(MULTI_ROCKET.equalsIgnoreCase(method)){
            MultiRocketParams params = new MultiRocketParams();
            if(args.length >= 6){
                params.rigIds = args[1];
                params.miner = args[2];
                params.miner2 = args[3];
                params.idWal = args[4];
                params.idOc = args[5];
                result = multiRocket(params);
            }else{
                // To use rocket you have to know what you are doing. Then delete these lines and edit the following.
                log("Please edit the source to use multiRocket method");
                return;

                //this is just an example, use some of your real ids which you get with other methods

                //set everything to rigs ids 1, 2 and 3
                //params.rigIds = "1,2,3";
                //params.miner = "claymore";
                //params.miner2 = "xmrig";
                //params.idWal = "107800";
                //params.idOc = "800";
                //result = multiRocket(params);

                //set bminer to rigs ids 4 and 5, unset second miner
                //params.rigIds = "4,5";
                //params.miner = "bminer";
                //params.miner2 = "0";
                //params.idWal = null;
                //params.idOc = null;
                //result = multiRocket(params);
            }
        } else {
            printHelp();
            return;
        }
        if(result != null) {
            log(result.toString(4));
        }
    }

    /**
     * Rigs list
     * @return
     */
    static JSONObject getRigs(){
        Map<String, String> httpParams = new HashMap<>();
        httpParams.put("method", GET_RIGS);
        Response result = sendPOST(HTTPS_URL, httpParams);
        if(result.responseCode == 200) {
            return ((JSONObject) result.responseData.get("result"));
        }else{
            return null;
        }
    }

    /**
     * Wallets list
     * @return
     */
    static JSONObject getWallets(){
        Map<String, String> httpParams = new HashMap<>();
        httpParams.put("method", GET_WALLETS);
        Response result = sendPOST(HTTPS_URL, httpParams);
        if(result.responseCode == 200) {
            return ((JSONObject) result.responseData.get("result"));
        }else{
            return null;
        }
    }

    /**
     * Overclocking profiles
     * @return
     */
    static JSONObject getOC(){
        Map<String, String> httpParams = new HashMap<>();
        httpParams.put("method", GET_OC);
        Response result = sendPOST(HTTPS_URL, httpParams);
        if(result.responseCode == 200) {
            return ((JSONObject) result.responseData.get("result"));
        }else{
            return null;
        }
    }

    /**
     * Monitor stats for all the rigs
     * @return
     */
    static JSONObject getCurrentStats(){
        Map<String, String> httpParams = new HashMap<>();
        httpParams.put("method", GET_CURRENT_STATS);
        Response result = sendPOST(HTTPS_URL, httpParams);
        if(result.responseCode == 200) {
            return ((JSONObject) result.responseData.get("result"));
        }else{
            return null;
        }
    }

    /**
     * Sets parameters for rigs
     * @param params see MultiRocketParams
     * @return
     */
    static JSONObject multiRocket(MultiRocketParams params){
        Map<String, String> httpParams = new HashMap<>();
        httpParams.put("method", MULTI_ROCKET);
        if(params.rigIds != null) httpParams.put("rig_ids_str", params.rigIds);
        if(params.miner != null) httpParams.put("miner", params.miner);
        if(params.miner2 != null) httpParams.put("miner2", params.miner2);
        if(params.idWal != null) httpParams.put("id_wal", params.idWal);
        if(params.idOc != null) httpParams.put("id_oc", params.idOc);
        Response result = sendPOST(HTTPS_URL, httpParams);
        if(result.responseCode == 200) {
            return ((JSONObject) result.responseData.get("result"));
        }else{
            return null;
        }
    }

    /**
     * Make API request with given params. Signs the request with secret key.
     * @param params
     * @return
     */
    static Response sendPOST(String url, Map<String, String> params) {
        params.put("public_key", PUBLIC_KEY);
        String urlParameters = buildQueryString(params, UTF_8);
        Response response = new Response();
        StringBuffer buf = new StringBuffer();

        URL obj = null;
        try {
            obj = new URL(url);
            HttpsURLConnection con = (HttpsURLConnection) obj.openConnection();

            // add request header
            con.setRequestProperty("HMAC", encodeHMAC(SECRET_KEY, urlParameters));
            con.setRequestMethod("POST");
            con.setDoOutput(true);
            con.setDoInput(true);

            // Send post request
            DataOutputStream wr = new DataOutputStream(con.getOutputStream());
            wr.writeBytes(urlParameters);
            wr.flush();
            wr.close();

            // Get result
            response.responseCode = con.getResponseCode();
            InputStream is;
            if(response.responseCode == 200) {
                is = con.getInputStream();
            }else{
                is = con.getErrorStream();
            }
            BufferedReader in = new BufferedReader(new InputStreamReader(is));
            String inputLine;
            while ((inputLine = in.readLine()) != null) {
                buf.append(inputLine);
            }
            in.close();
            if(response.responseCode == 200) {
                response.responseData = new JSONObject(buf.toString());
            }else{
                try{
                    response.responseData = new JSONObject(buf.toString());
                    log("ERROR: HTTP " + response.responseCode + ": \n" + response.responseData.toString(4));
                }catch (JSONException e) {
                    log("ERROR: HTTP " + response.responseCode + ": " + buf.toString());
                }
            }
        } catch (MalformedURLException e) {
            log("ERROR: Invalid URL: " + url);
            e.printStackTrace();
        } catch (ProtocolException e) {
            log("ERROR: Invalid request method");
            e.printStackTrace();
        } catch (IOException e) {
            log("ERROR: input/output operation");
            e.printStackTrace();
        } catch (JSONException e) {
            log("ERROR: Invalid json response: " + buf.toString());
            e.printStackTrace();
        } catch (NoSuchAlgorithmException e) {
            log("ERROR: Can't find HmacSHA256 algorithm");
            e.printStackTrace();
        } catch (InvalidKeyException e) {
            log("ERROR: Invalid secret key");
            e.printStackTrace();
        }
        return response;
    }

    static String encodeHMAC(String key, String data) throws UnsupportedEncodingException, NoSuchAlgorithmException, InvalidKeyException {
        Mac sha256_HMAC = Mac.getInstance("HmacSHA256");
        SecretKeySpec secret_key = new SecretKeySpec(key.getBytes(UTF_8), "HmacSHA256");
        sha256_HMAC.init(secret_key);

        return 	DatatypeConverter.printHexBinary(sha256_HMAC.doFinal(data.getBytes(UTF_8))).toLowerCase();
    }

    static String buildQueryString(Map<String, String> parameters, String encoding) {
        return parameters.entrySet().stream()
                .map(entry -> encodeParameter(entry.getKey(), entry.getValue(), encoding))
                .reduce((param1, param2) -> param1 + "&" + param2)
                .orElse("");
    }

    static String encodeParameter(String key, String value, String encoding) {
        return urlEncode(key, encoding) + "=" + urlEncode(value, encoding);
    }

    static String urlEncode(String value, String encoding) {
        try {
            return URLEncoder.encode(value, encoding);
        } catch (UnsupportedEncodingException e) {
            throw new IllegalArgumentException("Cannot url encode " + value, e);
        }
    }

}
