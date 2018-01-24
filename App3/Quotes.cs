using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonySym
{
    class Quotes
    {
        private List<string> lines = new List<string>();
        private Random rand;

        public Quotes()
        {
            rand = new Random();
            LoadQuotes();
        }

        private void LoadQuotes()
        {
            lines.Add("If at first you don't succeed; call it version 1.0");
            lines.Add("The Internet: where men are men, women are men, and children are FBI agents.");
            lines.Add("Some things Man was never meant to know. For everything else, there's Google.");
            lines.Add("unzip; strip; touch; finger; mount; fsck; more; yes; unmount; sleep'  - my daily unix command list");
            lines.Add("... one of the main causes of the fall of the Roman Empire was that, lacking zero, they had no way to indicate successful termination of their C programs.' - Robert Firth");
            lines.Add("If Python is executable pseudocode, then perl is executable line noise.");
            lines.Add("The more I C, the less I see.");
            lines.Add("To err is human... to really foul up requires the root password.");
            lines.Add("After Perl everything else is just assembly language.");
            lines.Add("If brute force doesn't solve your problems, then you aren't using enough.");
            lines.Add("Life would be so much easier if we only had the source code.");
            lines.Add("Unix is user-friendly. It's just very selective about who its friends are.");
            lines.Add("COBOL programmers understand why women hate periods.");
            lines.Add("Programming is like sex, one mistake and you have to support it for the rest of your life.” — Michael Sinz");
            lines.Add("There are 10 types of people in the world: those who understand binary, and those who don't.");
            lines.Add("640K ought to be enough for anybody.' - This is not humorous by itself; but in the context it's a classic by Bill Gates in 1981");
            lines.Add("Microsoft: 'You've got questions. We've got dancing paperclips.");
            lines.Add("Microsoft is not the answer. Microsoft is the question. NO is the answer.'   - Erik Naggum");
            lines.Add("Men are from Mars. Women are from Venus. Computers are from hell.");
            lines.Add("SUPERCOMPUTER: what it sounded like before you bought it.");
            lines.Add("Windows95: It's like upgrading from Reagan to Bush.");
            lines.Add("People say Microsoft paid 14M$ for using the Rolling Stones song 'Start me up' in their commercials. This is wrong. Microsoft payed 14M$ only for a part of the song. For instance, they didn't use the line 'You'll make a grown man cry'.");
            lines.Add("I'm not anti-social; I'm just not user friendly");
            lines.Add("A printer consists of three main parts: the case, the jammed paper tray and the blinking red light");
            lines.Add("The best accelerator available for a Mac is one that causes it to go at 9.81 m/s2.");
            lines.Add("A computer lets you make more mistakes faster than any invention in human history - with the possible exceptions of handguns and tequila");
            lines.Add("1f u c4n r34d th1s u r34lly n33d t0 g37 l41d");
            lines.Add("To go forward, you must backup.");
            lines.Add("I would love to change the world, but they won't give me the source code");
            lines.Add("A Windows user spends 1/3 of his life sleeping, 1/3 working, 1/3 waiting.");
            lines.Add("My software never has bugs. It just develops random features.");
            lines.Add("Better to be a geek than an idiot.");
            lines.Add("Windows isn't a virus, viruses do something.");
            lines.Add("Geek's favorite pickup line: Hey, does this rag smell like chloroform?");
            lines.Add("Be nice to geeks when you're in school, you might end-up working for one when you grow-up.");
            lines.Add("Difference between a virus and windows ? Viruses rarely fail.");
            lines.Add("Evolution is God's way of issuing upgrades.");
            lines.Add("The only problem with troubleshooting is that sometimes trouble shoots back.");
            lines.Add("It's a little-known fact that the Y1K problem caused the Dark Ages.");
            lines.Add("The box said 'Required Windows 95 or better'. So, I installed LINUX.");
            lines.Add("Computer are like air conditioners: they stop working when you open windows.");
            lines.Add("Mac users swear by their Mac, PC users swear at their PC.");
            lines.Add("Like car accidents, most hardware problems are due to driver error.");
            lines.Add("Dating a girl is just like writing software. Everything's going to work just fine in the testing lab (dating), but as soon as you have contract with a customer (marriage), then your program (life) is going to be facing new situations you never expected. You'll be forced to patch the code (admit you're wrong) and then the code (wife) will just end up all bloated and unmaintainable in the end.");
            lines.Add("Real men don't use backups, they post their stuff on a public ftp server and let the rest of the world make copies.'  - Linus Torvalds");
            lines.Add("There are 10 kinds of people in the world, those that understand trinary, those that don't, and those that confuse it with binary.");
            lines.Add("If you give someone a program, you will frustrate them for a day; if you teach them how to program, you will frustrate them for a lifetime.");
            lines.Add("It is easier to change the specification to fit the program than vice versa.");
            lines.Add("I had a fortune cookie the other day and it said: 'Outlook not so good'. I said: 'Sure, but Microsoft ships it anyway'.");
            lines.Add("The nice thing about standards is that there are so many to choose from.");
            lines.Add("The term reboot comes from the middle age (before computers). Horses who stopped in mid-stride required a boot to the rear to start again. Thus the term to rear-boot, later abbreviated into reboot.");
            lines.Add("Programmers are tools for converting caffeine into code.");
            lines.Add("The great thing about Object Oriented code is that it can make small, simple problems look like large, complex ones.");
            lines.Add("Hacking is like sex. You get in, you get out, and hope that you didn't leave something that can be traced back to you.");
            lines.Add("People say that if you play Microsoft CD's backwards, you hear satanic things, but that's nothing, because if you play them forwards, they install Windows.");
            lines.Add("The box said 'Requires Windows 95 or better'. So I installed LINUX.");
            lines.Add("In a world without fences and walls, who needs Gates and Windows?");
            lines.Add("MICROSOFT = Most Intelligent Customers Realize Our Software Only Fools Teenagers");
            lines.Add("Windows has detected you do not have a keyboard. Press 'F9' to continue.");
            lines.Add("Software is like sex: It's better when it's free.");
            lines.Add("Unix, DOS and Windows...the good, the bad and the ugly.");
            lines.Add("ACs are like computers- Both work fine until you open Windows!");
        }

        public string GetQuote()
        {
            int index = rand.Next(0, lines.Count);
            return lines.ElementAt(index);
        }
    }
}
