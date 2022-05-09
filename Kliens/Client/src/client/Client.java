/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package client;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.UnknownHostException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.JOptionPane;
import javax.swing.ListModel;
import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.JSONValue;

/**
 *
 * @author Ákos
 */
public class Client {

    // initialize socket and input output streams
    private Socket socket = null;
    private BufferedReader input = null;
    private PrintWriter out = null;

    //Hash
    private String hash, name, role;

    public String getRole() {
        return role;
    }

    public String getName() {
        return name;
    }

    // constructor to put ip address and port
    public Client(String address, int port) {
        // establish a connection
        try {
            socket = new Socket(address, port);
            System.out.println("Connected to: " + socket.getInetAddress().getHostAddress());

            // takes input from terminal
            input = new BufferedReader(new InputStreamReader(socket.getInputStream()));

            // sends output to the socket
            out = new PrintWriter(socket.getOutputStream(), true);

        } catch (UnknownHostException u) {
            error(u.getLocalizedMessage());
        } catch (IOException i) {
            error(i.getLocalizedMessage());
        }
    }

    public Object sendAndRecieveJSON(JSONObject JSON) {
        String JSONtext = JSON.toJSONString(), JSONreply = null;
        try {
            System.out.println(JSONtext);
            out.print(JSONtext);
            out.flush();
            JSONreply = input.readLine();
            System.out.println(JSONreply);
        } catch (IOException ex) {
            Logger.getLogger(Client.class.getName()).log(Level.SEVERE, null, ex);
        }
        return JSONValue.parse(JSONreply);
    }

    public JSONArray getAllSchool(){
        JSONObject obj = new JSONObject();
        JSONArray ret = null;
        obj.put("code", 11);
        obj.put("hash", hash);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            ret = (JSONArray)obj.get("kepesites");
        }
        System.out.println(ret);
        return ret;
        
    }
    public JSONArray getUserSchools(String username){
        JSONObject obj = new JSONObject();
        JSONArray ret = null;
        obj.put("code", 22);
        obj.put("hash", hash);
        obj.put("username", username);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            ret = (JSONArray)obj.get("kepesites");
        }
        System.out.println(ret);
        return ret;
    }
    
    public boolean SignIn(String username, char[] password) {        
        JSONObject obj = new JSONObject();
        obj.put("code", 1);
        obj.put("username", username);
        obj.put("password", encrypt(password));
        
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            hash = (String) obj.get("hash");
            name = (String) obj.get("name");
            role = (String) obj.get("role");
        }
        return state;
    }
    
    public boolean addNewMaintenance(int toolid, String date, String description){
        JSONObject obj = new JSONObject();
        obj.put("hash", hash);
        obj.put("code", 14);
        obj.put("eszkozid", toolid);
        obj.put("date", date);
        obj.put("leiras", description);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        return state;
    }

    public boolean addMember(String name, String username, char[] password, String role) {
        JSONObject obj = new JSONObject();
        obj.put("hash", hash);
        obj.put("code", 5);
        obj.put("name", name);
        obj.put("username", username);
        obj.put("password", encrypt(password));
        obj.put("role", role);
        
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        return state;
    }
    
    public boolean addSchoolToUser(String username, String name) {
        JSONObject obj = new JSONObject();
        obj.put("hash", hash);
        obj.put("code", 20);
        obj.put("name", name);
        obj.put("username", username);
        
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        return state;
    }
    
    public boolean addJobToMaintenance(int karbantartoid, int karbantartasid, int ido) {
        JSONObject obj = new JSONObject();
        obj.put("hash", hash);
        obj.put("code", 15);
        obj.put("karbantartoid", karbantartoid);
        obj.put("karbantartasid", karbantartasid);
        obj.put("ido", ido);
        
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        return state;
    }
    
    public boolean addItem(String id, String name, int category, String location, String description) {
        JSONObject obj = new JSONObject();
        obj.put("code", 6);
        obj.put("hash", hash);
        obj.put("azonosito", id);        
        obj.put("name", name);
        obj.put("kategoriaid", category);
        obj.put("elhelyezkedes", location);
        obj.put("leiras", description);
        
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        return state;
    }
    
    public boolean addCategory(String name, String category, Integer normaido, String period, String instructions) {
        JSONObject obj = new JSONObject();
        obj.put("code", 3);
        obj.put("hash", hash);
        obj.put("name", name);        
        obj.put("parent", category);
        obj.put("normaido", normaido);
        obj.put("karbperiod", period);
        obj.put("leiras", instructions);
        
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        return state;
    }
    
    public JSONArray getCategorys(){
        JSONObject obj = new JSONObject();
        JSONArray ret = null;
        obj.put("hash", hash);
        obj.put("code", 4);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            ret = (JSONArray)obj.get("kategoria");                  
        }
        
        return ret;
    }
    
    public JSONArray getUsers(int eszkozid, int karbantartasid){
        JSONObject obj = new JSONObject();
        JSONArray ret = null;
        obj.put("hash", hash);
        obj.put("code", 9);
        obj.put("eszkozid", eszkozid);
        obj.put("karbantartasid", karbantartasid);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            ret = (JSONArray)obj.get("felhasznalo");          
        }        
        return ret;
    }
    
    public JSONArray getTools(){
        JSONObject obj = new JSONObject();
        JSONArray ret = null;
        obj.put("hash", hash);
        obj.put("code", 8);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            ret = (JSONArray)obj.get("eszkoz");          
        }        
        return ret;
    }
    
    public boolean assignQualification(String qualification){
        //TYŰ, ezt azért lehet, hogy újra kéne gondolni.
        String JSONtext;
        JSONObject JSONreply;
        JSONObject obj = new JSONObject();
        obj.put("hash", hash);
        obj.put("code", 7);
        obj.put("name", qualification);
        StringBuilder str = new StringBuilder();
        str.append("[");
        ListModel<String> model = null; //jList2.getModel(); volt ott
        for(int i=0;i<model.getSize();i++){
            if(i!=model.getSize()-1)
            {
                str.append('"');
                str.append(model.getElementAt(i));
                str.append('"');
                str.append(',');
            }   
            else
            {
                str.append('"');
                str.append(model.getElementAt(i));
                str.append('"');
            }   
               
        }
        str.append("]");
        
        JSONtext = "{\"kategoriaaz\":" + str.toString() + ",\"code\":7,\"hash\":\"" +hash + "\",\"name\":\"" +qualification +"\"}";
        System.out.println(JSONtext);
        
        JSONreply = (JSONObject)sendAndRecieveJSON(new JSONObject()); //JSONtext volt a paramétere.
        return true;
    }

    public JSONArray getTODoList(int karbantartoid){
        JSONObject obj = new JSONObject();
        JSONArray ret = null;
        obj.put("code", 10);
        obj.put("hash", hash);
        obj.put("karbantartoid", karbantartoid);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            ret = (JSONArray)obj.get("karbantartas");                  
        }else{
            return null;
        }
        return ret;
    }
    
    public int acceptRepair(int karbantartasId){
        JSONObject obj = new JSONObject();
        obj.put("id", karbantartasId);
        obj.put("hash", hash);
        obj.put("code", 16);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        System.out.println(state);
        return Integer.parseInt(obj.get("state").toString());
    }
    
    public int rejectRepair(int karbantartasId){
        JSONObject obj = new JSONObject();
        obj.put("id", karbantartasId);
        obj.put("hash", hash);
        obj.put("code", 17);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        System.out.println(state);
        return Integer.parseInt(obj.get("state").toString());
    }
    
    public String startRepair(int karbantartasId){
        String leiras = "";
        JSONObject obj = new JSONObject();
        obj.put("id", karbantartasId);
        obj.put("hash", hash);
        obj.put("code", 18);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            leiras = obj.get("leiras").toString();                  
        }else{
            return null;
        }
        return leiras;
    }
    
    public int finishRepair(int karbantartasId){
        JSONObject obj = new JSONObject();
        obj.put("id", karbantartasId);
        obj.put("hash", hash);
        obj.put("code", 19);
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        System.out.println(state);
        return Integer.parseInt(obj.get("state").toString());
    }
    
    public void LogOut() {
        JSONObject obj = new JSONObject();
        obj.put("code", 2);
        obj.put("hash", hash);
        
        obj = (JSONObject)sendAndRecieveJSON(obj);
        boolean state = (Long) (obj.get("state")) == 0;
        if (state) {
            hash = null;
            name = null;
            role = null;
        }
    }

    private String encrypt(char[] password) {

        /* MessageDigest instance for MD5. */
        MessageDigest m = null;
        try {
            m = MessageDigest.getInstance("MD5");
        } catch (NoSuchAlgorithmException ex) {
            Logger.getLogger(Client.class.getName()).log(Level.SEVERE, null, ex);
        }

        /* Add plain-text password bytes to digest using MD5 update() method. */
        m.update(String.copyValueOf(password).getBytes());

        /* Convert the hash value into bytes */
        byte[] bytes = m.digest();

        /* The bytes array has bytes in decimal form. Converting it into hexadecimal format. */
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < bytes.length; i++) {
            s.append(Integer.toString((bytes[i] & 0xff) + 0x100, 16).substring(1));
        }

        /* Complete hashed password in hexadecimal format */
        return s.toString();
    }

    private void error(String msg) {
        JOptionPane.showMessageDialog(null, msg, "Szerver nem elérhető", JOptionPane.ERROR_MESSAGE);
        System.exit(0);
    }

    public void close() {
        // close the connection
        try {
            input.close();
            out.close();
            socket.close();
        } catch (IOException i) {
            System.out.println(i);
        }
    }
}
