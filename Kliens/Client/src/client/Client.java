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
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.JOptionPane;
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

            input = new BufferedReader(new InputStreamReader(socket.getInputStream()));

            // sends output to the socket
            out = new PrintWriter(socket.getOutputStream(), true);            
            
        } catch (UnknownHostException u) {
            error(u.getLocalizedMessage());
        } catch (IOException i) {
            error(i.getLocalizedMessage());
        }
    }

    public boolean SignIn(String username, char[] password) {
        String JSONtext, JSONreply = "";
        JSONObject obj = new JSONObject();
        obj.put("code", 1);
        obj.put("username", username);
        obj.put("password", encrypt(password));
        JSONtext = obj.toJSONString();
        try {
            out.print(JSONtext);
            out.flush();
            JSONreply = input.readLine();
        } catch (IOException ex) {
            Logger.getLogger(Client.class.getName()).log(Level.SEVERE, null, ex);
        }

        obj = (JSONObject)JSONValue.parse(JSONreply);
        boolean state = (Long)(obj.get("state"))==0;
        if (state){
            hash = (String)obj.get("hash");
            name = (String)obj.get("name");
            role = (String)obj.get("role");
        }
        return state;
    }
    
    public void LogOut(){
        String JSONtext, JSONreply = "";
        JSONObject obj = new JSONObject();
        obj.put("code", 2);
        obj.put("hash", hash);        
        JSONtext = obj.toJSONString();
        try {
            out.print(JSONtext);
            out.flush();
            JSONreply = input.readLine();
        } catch (IOException ex) {
            Logger.getLogger(Client.class.getName()).log(Level.SEVERE, null, ex);
        }
        obj = (JSONObject)JSONValue.parse(JSONreply);
        if ((Long)obj.get("state")==0){
            hash = null;
            name = null;
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
