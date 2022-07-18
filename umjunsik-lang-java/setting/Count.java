package setting;

import java.util.ArrayList;
import java.util.List;

public class Count {
    int totalNumber = 0;

    //계산 후 반환
    public int count(String line) {
        totalNumber = 0;
        String[] texts = line.split(" ");
        List<Integer> list = new ArrayList<>();

        for (String text : texts) {
            int count = 0;
            for (int i = 0; i<text.length(); i++) {
                char c = text.charAt(i);
                if (c=='.') ++count;
                else if (c==',') --count;
            } list.add(count);

        }

        totalNumber = list.get(0);
        list.remove(0);
        list.forEach(number -> totalNumber *= number);
        return totalNumber;
    }
}
