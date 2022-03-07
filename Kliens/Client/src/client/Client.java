/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package client;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;
import javax.swing.JOptionPane;

/**
 *
 * @author Ákos
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
            socket = new Socket(address, port);
//            System.out.println("Connected");
            new SignIn().setVisible(true);

            // takes input from terminal
            input = new DataInputStream(System.in);

            // sends output to the socket
            out = new DataOutputStream(socket.getOutputStream());
        } catch (UnknownHostException u) {
            JOptionPane.showMessageDialog(null, "Nem sikerült kapcsolódni a szerverhez... Próbáld újra később", "Szerver nem elérhető", JOptionPane.ERROR_MESSAGE);
            System.exit(0);
        } catch (IOException i) {
            JOptionPane.showMessageDialog(null, "Nem sikerült kapcsolódni a szerverhez... Próbáld újra később", "Szerver nem elérhető", JOptionPane.ERROR_MESSAGE);
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
        final String IP = "127.0.0.1";
        final int PORT = 5000;
        
        Client client = new Client(IP, PORT);
    }

}
