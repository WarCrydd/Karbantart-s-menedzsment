/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package client.marton;

import client.Client;
import client.akos.SignIn;
import java.util.ArrayList;
import java.util.List;
import javax.swing.JOptionPane;
import javax.swing.table.DefaultTableModel;
import org.json.simple.JSONArray;
import org.json.simple.JSONObject;

/**
 *
 * @author vassv
 */
public class Karbantarto extends javax.swing.JFrame {

    private Client client;
    private List<JSONObject> karbanTartasok = new ArrayList();
    private String feladatLeiras = "";
    
    public Karbantarto(Client c) {
        initComponents();
        //nev.setText(client.getName());
        this.setLocationRelativeTo(null);
        this.client = c;
        JSONArray obj = client.getTODoList(-1);
        if(obj == null){
            JOptionPane.showMessageDialog(rootPane, "Hiba", "Nem sikerült a feladatok betöltése, jelentkezz be újra", JOptionPane.INFORMATION_MESSAGE);
            client.LogOut();
            new SignIn(client).setVisible(true);
            dispose();
        }else{
            System.out.println(obj.toJSONString());
            listaFeltolt(obj);
            tablazatGeneralo(karbanTartasok);
            elfogad.setEnabled(false);
            elutasit.setEnabled(false);
            kezdesBtn.setEnabled(false);
            befejezes.setEnabled(false);
        }
        
    }
    
    public void listaFeltolt(JSONArray tomb){
        for (int i = 0; i < tomb.size(); i++) {
            JSONObject obj = (JSONObject)tomb.get(i);
            karbanTartasok.add(obj);
        }
    }

    public void tablazatGeneralo(List karbanTartasok){
        this.napiFeladatok.removeAll();
        String[] titles = {"eszköz neve", "súlyosság", "helyszín", "feladat állapota"};
        String[][] datas = new String[karbanTartasok.size()][4];
        for(int i = 0; i < karbanTartasok.size(); i++){
            JSONObject tmp = (JSONObject)karbanTartasok.get(i);
            datas[i][0] = tmp.get("name").toString();
            datas[i][1] = tmp.get("sulyossag").toString();
            datas[i][2] = tmp.get("helyszin").toString();
            datas[i][3] = tmp.get("allapot").toString();
        }
        this.napiFeladatok.setModel(new DefaultTableModel(datas, titles));
    }
    
    /**
     * This method is called from within the constructor to initialize the form.
     * WARNING: Do NOT modify this code. The content of this method is always
     * regenerated by the Form Editor.
     */
    @SuppressWarnings("unchecked")
    // <editor-fold defaultstate="collapsed" desc="Generated Code">//GEN-BEGIN:initComponents
    private void initComponents() {

        nev = new javax.swing.JLabel();
        jLabel2 = new javax.swing.JLabel();
        kijelentkezes = new javax.swing.JButton();
        jScrollPane1 = new javax.swing.JScrollPane();
        napiFeladatok = new javax.swing.JTable();
        jLabel1 = new javax.swing.JLabel();
        elfogad = new javax.swing.JButton();
        elutasit = new javax.swing.JButton();
        jLabel3 = new javax.swing.JLabel();
        helyszin = new javax.swing.JLabel();
        kezdesBtn = new javax.swing.JButton();
        befejezes = new javax.swing.JButton();
        lepesek = new javax.swing.JButton();
        jButton1 = new javax.swing.JButton();

        setDefaultCloseOperation(javax.swing.WindowConstants.EXIT_ON_CLOSE);

        nev.setFont(new java.awt.Font("Times New Roman", 0, 18)); // NOI18N
        nev.setText("Név");

        jLabel2.setFont(new java.awt.Font("Times New Roman", 0, 18)); // NOI18N
        jLabel2.setText("Bejelentkezett karbantartó:");

        kijelentkezes.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        kijelentkezes.setText("Kijelentkezés");
        kijelentkezes.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                kijelentkezesActionPerformed(evt);
            }
        });

        napiFeladatok.setModel(new javax.swing.table.DefaultTableModel(
            new Object [][] {
                {null, null, null, null},
                {null, null, null, null},
                {null, null, null, null},
                {null, null, null, null}
            },
            new String [] {
                "Title 1", "Title 2", "Title 3", "Title 4"
            }
        ));
        napiFeladatok.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                napiFeladatokMouseClicked(evt);
            }
        });
        jScrollPane1.setViewportView(napiFeladatok);

        jLabel1.setFont(new java.awt.Font("Times New Roman", 0, 18)); // NOI18N
        jLabel1.setText("Napi feladatok");

        elfogad.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        elfogad.setText("Elfogad");
        elfogad.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                elfogadActionPerformed(evt);
            }
        });

        elutasit.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        elutasit.setText("Elutasit");
        elutasit.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                elutasitActionPerformed(evt);
            }
        });

        jLabel3.setFont(new java.awt.Font("Tahoma", 0, 18)); // NOI18N
        jLabel3.setText("A következő feladat helyszíne:");

        helyszin.setFont(new java.awt.Font("Tahoma", 0, 18)); // NOI18N
        helyszin.setText("Nincsen aktív feladat");

        kezdesBtn.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        kezdesBtn.setText("Kezdés");
        kezdesBtn.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                kezdesBtnActionPerformed(evt);
            }
        });

        befejezes.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        befejezes.setText("Befejezve");
        befejezes.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                befejezesActionPerformed(evt);
            }
        });

        lepesek.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        lepesek.setText("A feladatok elvégzésének lépései");
        lepesek.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                lepesekActionPerformed(evt);
            }
        });

        jButton1.setText("Frissítés");
        jButton1.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                jButton1ActionPerformed(evt);
            }
        });

        javax.swing.GroupLayout layout = new javax.swing.GroupLayout(getContentPane());
        getContentPane().setLayout(layout);
        layout.setHorizontalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.TRAILING)
                    .addGroup(layout.createSequentialGroup()
                        .addComponent(elfogad, javax.swing.GroupLayout.PREFERRED_SIZE, 180, javax.swing.GroupLayout.PREFERRED_SIZE)
                        .addGap(59, 59, 59)
                        .addComponent(elutasit, javax.swing.GroupLayout.PREFERRED_SIZE, 180, javax.swing.GroupLayout.PREFERRED_SIZE)
                        .addGap(80, 80, 80)
                        .addComponent(kezdesBtn, javax.swing.GroupLayout.PREFERRED_SIZE, 180, javax.swing.GroupLayout.PREFERRED_SIZE)
                        .addGap(56, 56, 56)
                        .addComponent(befejezes, javax.swing.GroupLayout.PREFERRED_SIZE, 180, javax.swing.GroupLayout.PREFERRED_SIZE)
                        .addContainerGap(javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                    .addGroup(layout.createSequentialGroup()
                        .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.TRAILING)
                            .addGroup(layout.createSequentialGroup()
                                .addComponent(jLabel3, javax.swing.GroupLayout.PREFERRED_SIZE, 260, javax.swing.GroupLayout.PREFERRED_SIZE)
                                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.RELATED)
                                .addComponent(helyszin, javax.swing.GroupLayout.PREFERRED_SIZE, 691, javax.swing.GroupLayout.PREFERRED_SIZE))
                            .addComponent(lepesek, javax.swing.GroupLayout.PREFERRED_SIZE, 358, javax.swing.GroupLayout.PREFERRED_SIZE))
                        .addContainerGap())))
            .addGroup(javax.swing.GroupLayout.Alignment.TRAILING, layout.createSequentialGroup()
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.TRAILING)
                    .addGroup(layout.createSequentialGroup()
                        .addGap(410, 410, 410)
                        .addComponent(jLabel1, javax.swing.GroupLayout.PREFERRED_SIZE, 141, javax.swing.GroupLayout.PREFERRED_SIZE)
                        .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.RELATED, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                        .addComponent(jButton1, javax.swing.GroupLayout.PREFERRED_SIZE, 119, javax.swing.GroupLayout.PREFERRED_SIZE))
                    .addGroup(layout.createSequentialGroup()
                        .addContainerGap()
                        .addComponent(jScrollPane1))
                    .addGroup(layout.createSequentialGroup()
                        .addGap(286, 286, 286)
                        .addComponent(nev, javax.swing.GroupLayout.PREFERRED_SIZE, 209, javax.swing.GroupLayout.PREFERRED_SIZE)
                        .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.RELATED, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                        .addComponent(kijelentkezes, javax.swing.GroupLayout.PREFERRED_SIZE, 180, javax.swing.GroupLayout.PREFERRED_SIZE)))
                .addGap(36, 36, 36))
            .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
                .addGroup(layout.createSequentialGroup()
                    .addGap(20, 20, 20)
                    .addComponent(jLabel2, javax.swing.GroupLayout.PREFERRED_SIZE, 209, javax.swing.GroupLayout.PREFERRED_SIZE)
                    .addContainerGap(740, Short.MAX_VALUE)))
        );
        layout.setVerticalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addGap(22, 22, 22)
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING, false)
                    .addComponent(kijelentkezes, javax.swing.GroupLayout.DEFAULT_SIZE, 66, Short.MAX_VALUE)
                    .addComponent(nev, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addGap(18, 18, 18)
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING, false)
                    .addComponent(jLabel1, javax.swing.GroupLayout.DEFAULT_SIZE, 44, Short.MAX_VALUE)
                    .addComponent(jButton1, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.UNRELATED)
                .addComponent(jScrollPane1, javax.swing.GroupLayout.PREFERRED_SIZE, 172, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addGap(22, 22, 22)
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.BASELINE)
                    .addComponent(elutasit, javax.swing.GroupLayout.PREFERRED_SIZE, 50, javax.swing.GroupLayout.PREFERRED_SIZE)
                    .addComponent(elfogad, javax.swing.GroupLayout.PREFERRED_SIZE, 50, javax.swing.GroupLayout.PREFERRED_SIZE)
                    .addComponent(kezdesBtn, javax.swing.GroupLayout.PREFERRED_SIZE, 50, javax.swing.GroupLayout.PREFERRED_SIZE)
                    .addComponent(befejezes, javax.swing.GroupLayout.PREFERRED_SIZE, 50, javax.swing.GroupLayout.PREFERRED_SIZE))
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
                    .addGroup(layout.createSequentialGroup()
                        .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.RELATED, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                        .addComponent(lepesek, javax.swing.GroupLayout.PREFERRED_SIZE, 70, javax.swing.GroupLayout.PREFERRED_SIZE))
                    .addGroup(layout.createSequentialGroup()
                        .addGap(27, 27, 27)
                        .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.BASELINE)
                            .addComponent(jLabel3, javax.swing.GroupLayout.PREFERRED_SIZE, 81, javax.swing.GroupLayout.PREFERRED_SIZE)
                            .addComponent(helyszin, javax.swing.GroupLayout.PREFERRED_SIZE, 82, javax.swing.GroupLayout.PREFERRED_SIZE))
                        .addGap(0, 93, Short.MAX_VALUE))))
            .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
                .addGroup(layout.createSequentialGroup()
                    .addGap(21, 21, 21)
                    .addComponent(jLabel2, javax.swing.GroupLayout.PREFERRED_SIZE, 66, javax.swing.GroupLayout.PREFERRED_SIZE)
                    .addContainerGap(520, Short.MAX_VALUE)))
        );

        pack();
    }// </editor-fold>//GEN-END:initComponents

    private void kijelentkezesActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_kijelentkezesActionPerformed
        client.LogOut();
        new SignIn(client).setVisible(true);
        dispose();
    }//GEN-LAST:event_kijelentkezesActionPerformed

    private void elfogadActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_elfogadActionPerformed
        JSONObject obj = (JSONObject)karbanTartasok.get(napiFeladatok.getSelectedRow());
        int state = client.acceptRepair(Integer.parseInt(obj.get("id").toString()));
        if(state == 0){
            obj.remove("allapot");
            obj.put("allapot", "Elfogadva");
            karbanTartasok.remove(napiFeladatok.getSelectedRow());
            karbanTartasok.add(obj);
            napiFeladatok.setValueAt("Elfogadva", napiFeladatok.getSelectedRow(), 3);
            client.acceptRepair(Integer.parseInt(obj.get("id").toString()));
            tablazatGeneralo(karbanTartasok);
            boolean vanElfogadott = false;
            int elfogadottIdx = -1;
            for (int i = 0; i < karbanTartasok.size(); i++) {
                JSONObject tmp = (JSONObject)karbanTartasok.get(i);
                if(tmp.get("allapot").toString().equals("Elfogadva") || tmp.get("allapot").toString().equals("Megkezdve")){
                    vanElfogadott = true;
                    if(elfogadottIdx == -1){
                        elfogadottIdx = i;
                        break;
                    }
                }
            }
            if(vanElfogadott == true){
                JSONObject tmp = (JSONObject)karbanTartasok.get(elfogadottIdx);
                helyszin.setText(tmp.get("helyszin").toString());
            }else{
                helyszin.setText("Nincsen aktív feladat");
            }
            elfogad.setEnabled(false);
            elutasit.setEnabled(false);
            kezdesBtn.setEnabled(false);
            befejezes.setEnabled(false);
        }else{
            JOptionPane.showMessageDialog(rootPane, "Nem sikerült a feladat elfogadása", "Hiba", JOptionPane.INFORMATION_MESSAGE);
        }
        
    }//GEN-LAST:event_elfogadActionPerformed

    private void napiFeladatokMouseClicked(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_napiFeladatokMouseClicked
        if(napiFeladatok.getSelectedRow() != -1){
            elfogad.setEnabled(true);
            elutasit.setEnabled(true);
            kezdesBtn.setEnabled(true);
            befejezes.setEnabled(true);
        }
    }//GEN-LAST:event_napiFeladatokMouseClicked

    private void elutasitActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_elutasitActionPerformed
        JSONObject obj = (JSONObject)karbanTartasok.get(napiFeladatok.getSelectedRow());
        int state = client.rejectRepair(Integer.parseInt(obj.get("id").toString()));
        if(state == 0){
            obj.remove("allapot");
            obj.put("allapot", "Elutasítva");
            karbanTartasok.remove(napiFeladatok.getSelectedRow());
            karbanTartasok.add(obj);
            napiFeladatok.setValueAt("Elutasítva", napiFeladatok.getSelectedRow(), 3);

            tablazatGeneralo(karbanTartasok);
            boolean vanElfogadott = false;
            int elfogadottIdx = -1;
            for (int i = 0; i < karbanTartasok.size(); i++) {
                JSONObject tmp = (JSONObject)karbanTartasok.get(i);
                if(tmp.get("allapot").toString().equals("Elfogadva") || tmp.get("allapot").toString().equals("Megkezdve")){
                    vanElfogadott = true;
                    if(elfogadottIdx == -1){
                        elfogadottIdx = i;
                        break;
                    }
                }
            }
            if(vanElfogadott == true){
                JSONObject tmp = (JSONObject)karbanTartasok.get(elfogadottIdx);
                helyszin.setText(tmp.get("helyszin").toString());
            }else{
                helyszin.setText("Nincsen aktív feladat");
            }
            elfogad.setEnabled(false);
            elutasit.setEnabled(false);
            kezdesBtn.setEnabled(false);
            befejezes.setEnabled(false);
        }else{
            JOptionPane.showMessageDialog(rootPane, "Nem sikerült a feladat elutasítása", "Hiba", JOptionPane.INFORMATION_MESSAGE);
        }
        
    }//GEN-LAST:event_elutasitActionPerformed

    private void kezdesBtnActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_kezdesBtnActionPerformed
        JSONObject obj = (JSONObject)karbanTartasok.get(napiFeladatok.getSelectedRow());
        String kezdes = client.startRepair(Integer.parseInt(obj.get("id").toString()));
        if(kezdes != null){
            obj.remove("allapot");
            obj.put("allapot", "Megkezdve");
            karbanTartasok.remove(napiFeladatok.getSelectedRow());
            karbanTartasok.add(obj);
            napiFeladatok.setValueAt("Megkezdve", napiFeladatok.getSelectedRow(), 3);
            feladatLeiras = kezdes;
            tablazatGeneralo(karbanTartasok);
            boolean vanElfogadott = false;
            int elfogadottIdx = -1;
            for (int i = 0; i < karbanTartasok.size(); i++) {
                JSONObject tmp = (JSONObject)karbanTartasok.get(i);
                if(tmp.get("allapot").toString().equals("Elfogadva") || tmp.get("allapot").toString().equals("Megkezdve")){
                    vanElfogadott = true;
                    if(elfogadottIdx == -1){
                        elfogadottIdx = i;
                    }
                }
            }
            if(vanElfogadott == true){
                JSONObject tmp = (JSONObject)karbanTartasok.get(elfogadottIdx);
                helyszin.setText(tmp.get("helyszin").toString());
            }else{
                helyszin.setText("Nincsen aktív feladat");
            }
            elfogad.setEnabled(false);
            elutasit.setEnabled(false);
            kezdesBtn.setEnabled(false);
            befejezes.setEnabled(false);
        }else{
            JOptionPane.showMessageDialog(rootPane, "Nem sikerült a feladat megkezdése", "Hiba", JOptionPane.INFORMATION_MESSAGE);
        }
        
        
    }//GEN-LAST:event_kezdesBtnActionPerformed

    private void befejezesActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_befejezesActionPerformed
        JSONObject obj = (JSONObject)karbanTartasok.get(napiFeladatok.getSelectedRow());
        int state = client.finishRepair(Integer.parseInt(obj.get("id").toString()));
        if(state == 0){
            obj.remove("allapot");
            obj.put("allapot", "Befejezve");
            karbanTartasok.remove(napiFeladatok.getSelectedRow());
            karbanTartasok.add(obj);
            napiFeladatok.setValueAt("Befejezve", napiFeladatok.getSelectedRow(), 3);
            tablazatGeneralo(karbanTartasok);
            boolean vanElfogadott = false;
            int elfogadottIdx = -1;
            for (int i = 0; i < karbanTartasok.size(); i++) {
                JSONObject tmp = (JSONObject)karbanTartasok.get(i);
                if(tmp.get("allapot").toString().equals("Elfogadva") || tmp.get("allapot").toString().equals("Megkezdve")){
                    vanElfogadott = true;
                    if(elfogadottIdx == -1){
                        elfogadottIdx = i;
                    }
                }
            }
            if(vanElfogadott == true){
                JSONObject tmp = (JSONObject)karbanTartasok.get(elfogadottIdx);
                helyszin.setText(tmp.get("helyszin").toString());
            }else{
                helyszin.setText("Nincsen aktív feladat");
            }
            elfogad.setEnabled(false);
            elutasit.setEnabled(false);
            kezdesBtn.setEnabled(false);
            befejezes.setEnabled(false);
            feladatLeiras = "";
        }else{
            JOptionPane.showMessageDialog(rootPane, "Nem sikerült a feladat befejezése", "Hiba", JOptionPane.INFORMATION_MESSAGE);
        }
        
    }//GEN-LAST:event_befejezesActionPerformed

    private void lepesekActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_lepesekActionPerformed
        JOptionPane.showMessageDialog(rootPane, feladatLeiras, "Lépések", JOptionPane.INFORMATION_MESSAGE);
    }//GEN-LAST:event_lepesekActionPerformed

    private void jButton1ActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_jButton1ActionPerformed
        JSONArray obj = client.getTODoList(-1);
        if(obj == null){
            JOptionPane.showMessageDialog(rootPane, "Hiba", "Nem sikerült a feladatok listáját frissíteni", JOptionPane.INFORMATION_MESSAGE);
        }else{
            karbanTartasok.clear();
            listaFeltolt(obj);
            tablazatGeneralo(karbanTartasok);
        }
    }//GEN-LAST:event_jButton1ActionPerformed

    /**
     * @param args the command line arguments
     */
    public static void main(String args[]) {
        /* Set the Nimbus look and feel */
        //<editor-fold defaultstate="collapsed" desc=" Look and feel setting code (optional) ">
        /* If Nimbus (introduced in Java SE 6) is not available, stay with the default look and feel.
         * For details see http://download.oracle.com/javase/tutorial/uiswing/lookandfeel/plaf.html 
         */
        try {
            for (javax.swing.UIManager.LookAndFeelInfo info : javax.swing.UIManager.getInstalledLookAndFeels()) {
                if ("Nimbus".equals(info.getName())) {
                    javax.swing.UIManager.setLookAndFeel(info.getClassName());
                    break;
                }
            }
        } catch (ClassNotFoundException ex) {
            java.util.logging.Logger.getLogger(Karbantarto.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        } catch (InstantiationException ex) {
            java.util.logging.Logger.getLogger(Karbantarto.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        } catch (IllegalAccessException ex) {
            java.util.logging.Logger.getLogger(Karbantarto.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        } catch (javax.swing.UnsupportedLookAndFeelException ex) {
            java.util.logging.Logger.getLogger(Karbantarto.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        }
        //</editor-fold>

        /* Create and display the form */
        java.awt.EventQueue.invokeLater(new Runnable() {
            public void run() {
                new Karbantarto(null).setVisible(true);
            }
        });
    }

    // Variables declaration - do not modify//GEN-BEGIN:variables
    private javax.swing.JButton befejezes;
    private javax.swing.JButton elfogad;
    private javax.swing.JButton elutasit;
    private javax.swing.JLabel helyszin;
    private javax.swing.JButton jButton1;
    private javax.swing.JLabel jLabel1;
    private javax.swing.JLabel jLabel2;
    private javax.swing.JLabel jLabel3;
    private javax.swing.JScrollPane jScrollPane1;
    private javax.swing.JButton kezdesBtn;
    private javax.swing.JButton kijelentkezes;
    private javax.swing.JButton lepesek;
    private javax.swing.JTable napiFeladatok;
    private javax.swing.JLabel nev;
    // End of variables declaration//GEN-END:variables
}
