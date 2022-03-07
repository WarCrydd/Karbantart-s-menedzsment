/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.client;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;

/**
 *
 * @author balazs
 */
public class Client {
    // initialize socket and input output streams
    private Socket socket = null;
    private DataInputStream input = null;
    private DataOutputStream out = null;

    // constructor to put ip address and port
    public Client(String address, int port) {
        // establish a connection
        try {
<<<<<<< HEAD:Kliens/Client/src/main/java/com/mycompany/client/Client.java
//            socket = new Socket(address, port);
//            System.out.println("Connected");
            new SignIn().setVisible(true);
=======
            socket = new Socket(address, port);
            System.out.println("Connected");
>>>>>>> parent of d824d9e (Kezdetleges bejelentkező ablak):Kliens/Client/src/client/Client.java

            // takes input from terminal
            input = new DataInputStream(System.in);

            // sends output to the socket
            out = new DataOutputStream(socket.getOutputStream());
        } catch (UnknownHostException u) {
            System.out.println(u);
            System.exit(0);
        } catch (IOException i) {
            System.out.println(i);
            System.exit(0);
        }

        // string to read message from input
        String line = "";

        // keep reading until "Over" is input
        while (!line.equals("Over")) {
            try {
                line = input.readUTF();
                out.writeUTF(line);
            } catch (IOException i) {
                System.out.println(i);
            }
        }

        close();
    }
    
    private void close(){
        // close the connection
        try {
            input.close();
            out.close();
            socket.close();
        } catch (IOException i) {
            System.out.println(i);
        }
    }
    
    /**
     * @param args the command line arguments
     */
    public static void main(String args[]) {
        Client client = new Client("127.0.0.1", 5000);
    }
}
