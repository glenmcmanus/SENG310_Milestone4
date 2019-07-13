//taken from https://answers.unity.com/questions/433283/how-to-send-email-with-c.html

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

[CreateAssetMenu(fileName = "MailLog", menuName = "Mail Log")]
public class mono_gmail : ScriptableObject
{
    public string sender = "";
    public string pass = "";
    public string recipient = "";

    [TextArea]
    public string log = "";

    [ContextMenu("Save log to disk")]
    public void SaveLogToDisk()
    {
        string path = Application.dataPath + "/Log_" + (System.DateTime.Today).ToString().Split(' ')[0] + ".txt";

        Debug.Log(path);
        if(File.Exists(path))
        {
            int counter = 1;
            while (File.Exists(path + "_" + counter))
            {
                counter++;
            }

            path += "_" + counter;
        }

        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);

        TextWriter tw = new StreamWriter(fs);
        tw.Write(log);
        tw.Close();
    }

    public void LogClick(GameObject clickable)
    {
        string log = "\nOnClick: " + clickable.name + " [child of ";
        string parent = clickable.transform.parent != null ? clickable.transform.parent.name : "None";
        log += parent + "]" + " at " + Time.timeSinceLevelLoad;

        this.log += log + "\n";
    }

    public void LogSearch()
    {
        string levels = "Levels: ";

        if (CourseSearch.instance.levels.Count > 0)
            levels += CourseSearch.instance.levels[0].ToString();
        for (int i = 1; i < CourseSearch.instance.levels.Count; i++)
        {
            levels += ", " + CourseSearch.instance.levels[i].ToString();
        }

        string subjects = "Subjects: ";

        if (CourseSearch.instance.subjects.Count > 0)
            levels += CourseSearch.instance.subjects[0].ToString();
        for (int i = 1; i < CourseSearch.instance.subjects.Count; i++)
        {
            subjects += ", " + CourseSearch.instance.subjects[i].ToString();
        }

        string keywords = "Keywords: ";

        if (CourseSearch.instance.keywordContentParent.transform.childCount > 0)
            levels += CourseSearch.instance.keywordContentParent.transform.GetChild(0).GetComponent<Text>().text;
        for (int i = 1; i < CourseSearch.instance.subjects.Count; i++)
        {
            levels += ", " + CourseSearch.instance.keywordContentParent.transform.GetChild(i).GetComponent<Text>().text;
        }

        log += "\n\nOnSearch: \n>Semester: " + CourseSearch.instance.semester.ToString()
                + "\n>" + levels + "\n>" + subjects + "\n>" + keywords;
    }

    public void LogSurvey(Survey survey)
    {
        log += "\n" + survey.name;
        for(int i = 0; i < survey.frames.Count; i++)
        {
            log += "\n" + i + " > " + survey.frames[i].question.text + "\n>> ";

            switch(survey.questions[i].questionType)
            {
                case QuestionType.multipleChoice:
                    foreach(Toggle t in survey.frames[i].toggleGroup.GetComponentsInChildren<Toggle>())
                    {
                        if(t.isOn)
                        {
                            log += t.GetComponentInChildren<Text>().text;
                            break;
                        }
                    }
                    break;
                case QuestionType.shortAnswer:
                    log += survey.frames[i].shortResponseField.text;
                    break;
                case QuestionType.longAnswer:
                    log += survey.frames[i].longResponseField.text;
                    break;
            }

        }

        log += "\n\nTime since app start: " + Time.timeSinceLevelLoad + "\n\n";
    }

    public void LogTaskCompletion(Task task)
    {
        log += "\n\nCompleted task: " + task.name + " at " + Time.timeSinceLevelLoad + "\n\n";
    }

    public void LogTimeSinceAppStart()
    {
        log += "\n\nTime since app start: " + Time.timeSinceLevelLoad + "\n\n";
    }

    public void SendMail(string subject, string body)
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(sender);
        mail.To.Add(recipient);
        mail.Subject = subject;
        mail.Body = body;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(sender, pass) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");
    }

    public void SendMail()
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(sender);
        mail.To.Add(recipient);
        mail.Subject = "BlueJam Log";

        mail.Body = log;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(sender, pass) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");
    }
}
