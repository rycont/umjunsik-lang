package setting.var;

import setting.Items;

public class VarSet {

    public void set(String line) {

        int position = line.indexOf("엄") + 1;
        line = line.substring(position).trim();
        Items.map.put(count(line), line);
    }

    public void set(String line, int value) {
        int position = line.indexOf("엄") + 1;
        line = line.substring(0, position).trim();
        String total = ",.";
        for (int i = 0; i<value; i++) total+=".";
        Items.map.put(count(line), total);
    }

    public boolean check(String line) {
        if (line.isBlank()) return false;
        line = line.trim().replaceAll("어", "");
        return line.startsWith("엄");
    }

    private int count(String lines) {
        int count = 1;
        if (!lines.contains("어")) return count;
        lines = lines.substring(0, lines.indexOf("엄")).trim();
        for (int i = 0; i<lines.length(); i++) {
            if (lines.charAt(i)=='어') count++;
        }
        return count;
    }
}
